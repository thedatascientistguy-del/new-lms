// API Configuration
const API_BASE_URL = 'https://localhost:7146';

// State Management
let currentToken = localStorage.getItem('token');
let currentUser = JSON.parse(localStorage.getItem('user') || 'null');
let tokenExpiryTime = localStorage.getItem('tokenExpiry');
let sessionCheckInterval = null;

// DOM Elements
const loginPage = document.getElementById('loginPage');
const signupPage = document.getElementById('signupPage');
const dashboardPage = document.getElementById('dashboardPage');
const myBooksSection = document.getElementById('myBooksSection');
const addBookSection = document.getElementById('addBookSection');
const notification = document.getElementById('notification');

// Console logging utility
function log(message, data = null, level = 'info') {
    const timestamp = new Date().toISOString();
    const prefix = `[${timestamp}] [${level.toUpperCase()}]`;
    
    if (data) {
        console.log(`${prefix} ${message}`, data);
    } else {
        console.log(`${prefix} ${message}`);
    }
}

// Initialize
document.addEventListener('DOMContentLoaded', () => {
    log('Application initialized');
    
    // Check if user is authenticated
    if (currentToken && isTokenValid()) {
        log('Valid token found, showing dashboard');
        showDashboard();
        startSessionMonitoring();
    } else {
        log('No valid token, showing login page');
        clearSession();
        showLogin();
    }
    
    setupEventListeners();
    setupRouteProtection();
});

// Route Protection - Prevent manual URL navigation
function setupRouteProtection() {
    log('Setting up route protection');
    
    // Prevent back/forward navigation to protected pages
    window.addEventListener('popstate', (e) => {
        log('Navigation detected via browser history');
        if (!currentToken || !isTokenValid()) {
            log('Unauthorized navigation attempt blocked', null, 'warn');
            e.preventDefault();
            clearSession();
            showLogin();
            showNotification('Please login to access the application', 'error');
        }
    });
    
    // Prevent direct page access via URL manipulation
    window.addEventListener('hashchange', (e) => {
        log('Hash change detected');
        if (!currentToken || !isTokenValid()) {
            log('Unauthorized hash navigation blocked', null, 'warn');
            e.preventDefault();
            clearSession();
            showLogin();
        }
    });
}

// Session Management
function isTokenValid() {
    if (!tokenExpiryTime) {
        log('No token expiry time found', null, 'warn');
        return false;
    }
    
    const now = Date.now();
    const expiry = parseInt(tokenExpiryTime);
    const isValid = now < expiry;
    
    if (!isValid) {
        log('Token has expired', { now, expiry }, 'warn');
    }
    
    return isValid;
}

function startSessionMonitoring() {
    log('Starting session monitoring');
    
    // Check token validity every 30 seconds
    sessionCheckInterval = setInterval(() => {
        log('Checking session validity');
        
        if (!isTokenValid()) {
            log('Session expired, logging out', null, 'warn');
            clearInterval(sessionCheckInterval);
            handleSessionExpiry();
        } else {
            const timeLeft = Math.floor((parseInt(tokenExpiryTime) - Date.now()) / 1000 / 60);
            log(`Session valid, ${timeLeft} minutes remaining`);
        }
    }, 30000); // Check every 30 seconds
}

function stopSessionMonitoring() {
    if (sessionCheckInterval) {
        log('Stopping session monitoring');
        clearInterval(sessionCheckInterval);
        sessionCheckInterval = null;
    }
}

function handleSessionExpiry() {
    log('Handling session expiry', null, 'warn');
    showNotification('Your session has expired. Please login again.', 'error');
    clearSession();
    showLogin();
}

function clearSession() {
    log('Clearing session data');
    currentToken = null;
    currentUser = null;
    tokenExpiryTime = null;
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('tokenExpiry');
    stopSessionMonitoring();
}

function setSession(token, user) {
    log('Setting session data', { userId: user.userId, email: user.email });
    
    currentToken = token;
    currentUser = user;
    
    // Decode JWT to get expiry time
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        tokenExpiryTime = payload.exp * 1000; // Convert to milliseconds
        
        const expiryDate = new Date(tokenExpiryTime);
        log('Token expiry set', { expiryDate: expiryDate.toISOString() });
        
        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(user));
        localStorage.setItem('tokenExpiry', tokenExpiryTime.toString());
    } catch (error) {
        log('Failed to decode token', error, 'error');
    }
}

// Event Listeners
function setupEventListeners() {
    log('Setting up event listeners');
    
    // Login Form
    document.getElementById('loginForm').addEventListener('submit', handleLogin);
    
    // Signup Form
    document.getElementById('signupForm').addEventListener('submit', handleSignup);
    
    // Page Switching
    document.getElementById('showSignup').addEventListener('click', (e) => {
        e.preventDefault();
        log('User clicked signup link');
        showSignup();
    });
    
    document.getElementById('showLogin').addEventListener('click', (e) => {
        e.preventDefault();
        log('User clicked login link');
        showLogin();
    });
    
    // Logout
    document.getElementById('logoutBtn').addEventListener('click', handleLogout);
    
    // Navigation
    document.getElementById('navMyBooks').addEventListener('click', () => {
        log('User navigated to My Books');
        showMyBooks();
    });
    
    document.getElementById('navAddBook').addEventListener('click', () => {
        log('User navigated to Add Book');
        showAddBook();
    });
    
    // Book Form
    document.getElementById('bookForm').addEventListener('submit', handleSaveBook);
}

// API Calls
async function apiCall(endpoint, method = 'GET', body = null, encrypt = false) {
    log(`API Call: ${method} ${endpoint}`, body ? { bodySize: JSON.stringify(body).length } : null);
    
    // Check token validity before API call
    if (currentToken && !isTokenValid()) {
        log('Token expired before API call', null, 'warn');
        handleSessionExpiry();
        return null;
    }
    
    const headers = {
        'Content-Type': 'application/json'
    };
    
    if (currentToken) {
        headers['Authorization'] = `Bearer ${currentToken}`;
        log('Authorization header added');
    }
    
    const options = {
        method,
        headers
    };
    
    if (body) {
        const jsonBody = JSON.stringify(body);
        
        if (encrypt) {
            if (typeof encryptString !== 'function') {
                log('Encryption function not available', null, 'error');
                showNotification('Warning: Encryption not available, sending plain data', 'info');
                options.body = jsonBody;
            } else {
                try {
                    const encryptedBody = await encryptString(jsonBody);
                    options.body = encryptedBody;
                    headers['Content-Type'] = 'text/plain';
                    log('Payload encrypted successfully', { originalSize: jsonBody.length, encryptedSize: encryptedBody.length });
                } catch (encError) {
                    log('Encryption failed', encError, 'error');
                    showNotification('Encryption failed, sending plain data', 'info');
                    options.body = jsonBody;
                }
            }
        } else {
            options.body = jsonBody;
        }
    }
    
    try {
        const startTime = Date.now();
        const response = await fetch(`${API_BASE_URL}${endpoint}`, options);
        const duration = Date.now() - startTime;
        
        log(`API Response: ${response.status} ${response.statusText}`, { duration: `${duration}ms` });
        
        if (response.status === 401) {
            log('Unauthorized response, session expired', null, 'warn');
            handleSessionExpiry();
            return null;
        }
        
        if (!response.ok) {
            const error = await response.text();
            log('API Error Response', { status: response.status, error }, 'error');
            throw new Error(error || 'Request failed');
        }
        
        const contentType = response.headers.get('content-type');
        log('Response content type', { contentType });
        
        // Check if response is encrypted (text/plain) or plain JSON
        if (contentType && contentType.includes('text/plain')) {
            const encryptedText = await response.text();
            log('Encrypted response received', { size: encryptedText.length, preview: encryptedText.substring(0, 50) + '...' });
            
            if (encryptedText && typeof decryptString === 'function') {
                try {
                    const decryptedText = await decryptString(encryptedText);
                    log('Response decrypted successfully', { size: decryptedText.length, preview: decryptedText.substring(0, 100) });
                    const parsed = JSON.parse(decryptedText);
                    log('Response parsed successfully', parsed);
                    return parsed;
                } catch (decError) {
                    log('Decryption failed', decError, 'error');
                    log('Encrypted text that failed', { encryptedText: encryptedText.substring(0, 200) }, 'error');
                    throw new Error('Failed to decrypt response');
                }
            }
            
            return encryptedText;
        } else if (contentType && contentType.includes('application/json')) {
            const jsonData = await response.json();
            log('Plain JSON response received', jsonData);
            return jsonData;
        }
        
        const textData = await response.text();
        log('Plain text response received', { text: textData });
        return textData;
    } catch (error) {
        log('API Call failed', error, 'error');
        showNotification(error.message, 'error');
        return null;
    }
}

// Auth Handlers
async function handleLogin(e) {
    e.preventDefault();
    log('Login attempt started');
    
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;
    
    log('Login credentials collected', { email });
    
    try {
        const result = await apiCall('/api/auth/login', 'POST', { email, password }, true);
        
        log('Login API result received', result);
        
        if (result && result.token) {
            log('Login successful', { userId: result.userId });
            
            setSession(result.token, { userId: result.userId, email, username: result.username });
            
            showNotification('Login successful!', 'success');
            showDashboard();
            startSessionMonitoring();
        } else {
            log('Login failed - invalid response', { result }, 'warn');
            showNotification('Login failed. Please check your credentials.', 'error');
        }
    } catch (error) {
        log('Login error', error, 'error');
        showNotification('Login error: ' + error.message, 'error');
    }
}

async function handleSignup(e) {
    e.preventDefault();
    log('Signup attempt started');
    
    const username = document.getElementById('signupUsername').value;
    const email = document.getElementById('signupEmail').value;
    const password = document.getElementById('signupPassword').value;
    
    log('Signup data collected', { username, email });
    
    try {
        const result = await apiCall('/api/auth/signup', 'POST', { username, email, password }, true);
        
        log('Signup API result received', result);
        
        if (result && result.token) {
            log('Signup successful', { userId: result.userId });
            
            setSession(result.token, { userId: result.userId, email, username: result.username });
            
            showNotification('Signup successful!', 'success');
            showDashboard();
            startSessionMonitoring();
        } else {
            log('Signup failed - invalid response', { result }, 'warn');
            showNotification('Signup failed. Please try again.', 'error');
        }
    } catch (error) {
        log('Signup error', error, 'error');
        showNotification('Signup error: ' + error.message, 'error');
    }
}

function handleLogout() {
    log('User logout initiated');
    clearSession();
    showLogin();
    showNotification('Logged out successfully', 'success');
}

// Book Handlers
async function loadBooks() {
    log('Loading books');
    
    if (!currentToken || !isTokenValid()) {
        log('Cannot load books - not authenticated', null, 'warn');
        handleSessionExpiry();
        return;
    }
    
    const books = await apiCall('/api/books');
    
    if (books) {
        log('Books loaded successfully', { count: books.length });
        displayBooks(books);
    } else {
        log('Failed to load books', null, 'error');
    }
}

function displayBooks(books) {
    log('Displaying books', { count: books.length });
    const container = document.getElementById('booksContainer');
    
    if (!books || books.length === 0) {
        container.innerHTML = '<p class="no-books">No books found. Add your first book!</p>';
        return;
    }
    
    container.innerHTML = books.map(book => `
        <div class="book-card">
            <h3>${escapeHtml(book.title)}</h3>
            <p class="author">by ${escapeHtml(book.author)}</p>
            <p class="isbn">ISBN: ${escapeHtml(book.isbn)}</p>
            <p class="year">Published: ${book.publishedYear}</p>
            <div class="book-actions">
                <button class="btn btn-danger btn-sm" onclick="deleteBook(${book.id})">Delete</button>
            </div>
        </div>
    `).join('');
}

async function handleSaveBook(e) {
    e.preventDefault();
    log('Save book attempt started');
    
    if (!currentToken || !isTokenValid()) {
        log('Cannot save book - not authenticated', null, 'warn');
        handleSessionExpiry();
        return;
    }
    
    const book = {
        title: document.getElementById('bookTitle').value,
        author: document.getElementById('bookAuthor').value,
        isbn: document.getElementById('bookIsbn').value,
        publishedYear: parseInt(document.getElementById('bookPublishedYear').value)
    };
    
    log('Book data collected', book);
    
    try {
        const result = await apiCall('/api/books', 'POST', book, true);
        
        if (result) {
            log('Book added successfully', { bookId: result.id });
            showNotification('Book added successfully!', 'success');
            document.getElementById('bookForm').reset();
            showMyBooks();
            loadBooks();
        } else {
            log('Failed to add book', null, 'error');
            showNotification('Failed to add book', 'error');
        }
    } catch (error) {
        log('Add book error', error, 'error');
        showNotification('Error adding book: ' + error.message, 'error');
    }
}

async function deleteBook(id) {
    log('Delete book initiated', { bookId: id });
    
    if (!confirm('Are you sure you want to delete this book?')) {
        log('Delete cancelled by user');
        return;
    }
    
    if (!currentToken || !isTokenValid()) {
        log('Cannot delete book - not authenticated', null, 'warn');
        handleSessionExpiry();
        return;
    }
    
    const result = await apiCall(`/api/books/${id}`, 'DELETE');
    
    if (result !== null) {
        log('Book deleted successfully', { bookId: id });
        showNotification('Book deleted successfully!', 'success');
        loadBooks();
    } else {
        log('Failed to delete book', { bookId: id }, 'error');
    }
}

// UI Functions
function showLogin() {
    log('Showing login page');
    loginPage.classList.add('active');
    signupPage.classList.remove('active');
    dashboardPage.classList.remove('active');
    document.getElementById('loginForm').reset();
}

function showSignup() {
    log('Showing signup page');
    loginPage.classList.remove('active');
    signupPage.classList.add('active');
    dashboardPage.classList.remove('active');
    document.getElementById('signupForm').reset();
}

function showDashboard() {
    log('Showing dashboard');
    
    if (!currentToken || !isTokenValid()) {
        log('Cannot show dashboard - not authenticated', null, 'warn');
        handleSessionExpiry();
        return;
    }
    
    loginPage.classList.remove('active');
    signupPage.classList.remove('active');
    dashboardPage.classList.add('active');
    
    if (currentUser) {
        document.getElementById('userEmail').textContent = currentUser.email;
        log('User info displayed', { email: currentUser.email });
    }
    
    showMyBooks();
}

function showMyBooks() {
    log('Showing My Books section');
    
    if (!currentToken || !isTokenValid()) {
        log('Cannot show My Books - not authenticated', null, 'warn');
        handleSessionExpiry();
        return;
    }
    
    myBooksSection.classList.add('active');
    addBookSection.classList.remove('active');
    
    document.getElementById('navMyBooks').classList.add('active');
    document.getElementById('navAddBook').classList.remove('active');
    
    loadBooks();
}

function showAddBook() {
    log('Showing Add Book section');
    
    if (!currentToken || !isTokenValid()) {
        log('Cannot show Add Book - not authenticated', null, 'warn');
        handleSessionExpiry();
        return;
    }
    
    myBooksSection.classList.remove('active');
    addBookSection.classList.add('active');
    
    document.getElementById('navMyBooks').classList.remove('active');
    document.getElementById('navAddBook').classList.add('active');
    
    document.getElementById('bookForm').reset();
}

function showNotification(message, type = 'info') {
    log('Showing notification', { message, type });
    notification.textContent = message;
    notification.className = `notification ${type} show`;
    
    setTimeout(() => {
        notification.classList.remove('show');
    }, 3000);
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Log application ready
log('Application ready - all event listeners attached');
