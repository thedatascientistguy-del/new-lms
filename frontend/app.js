// API Configuration
const API_BASE_URL = 'https://localhost:7001'; // Update this port to match your API

// State Management
let currentToken = localStorage.getItem('token');
let currentUser = JSON.parse(localStorage.getItem('user') || 'null');

// DOM Elements
const loginPage = document.getElementById('loginPage');
const signupPage = document.getElementById('signupPage');
const dashboardPage = document.getElementById('dashboardPage');
const bookModal = document.getElementById('bookModal');
const notification = document.getElementById('notification');

// Initialize
document.addEventListener('DOMContentLoaded', () => {
    if (currentToken) {
        showDashboard();
    } else {
        showLogin();
    }
    
    setupEventListeners();
});

// Event Listeners
function setupEventListeners() {
    // Login Form
    document.getElementById('loginForm').addEventListener('submit', handleLogin);
    
    // Signup Form
    document.getElementById('signupForm').addEventListener('submit', handleSignup);
    
    // Page Switching
    document.getElementById('showSignup').addEventListener('click', (e) => {
        e.preventDefault();
        showSignup();
    });
    
    document.getElementById('showLogin').addEventListener('click', (e) => {
        e.preventDefault();
        showLogin();
    });
    
    // Logout
    document.getElementById('logoutBtn').addEventListener('click', handleLogout);
    
    // Add Book Button
    document.getElementById('addBookBtn').addEventListener('click', () => {
        openBookModal();
    });
    
    // Book Form
    document.getElementById('bookForm').addEventListener('submit', handleSaveBook);
    
    // Modal Close
    document.querySelector('.close').addEventListener('click', closeBookModal);
    window.addEventListener('click', (e) => {
        if (e.target === bookModal) {
            closeBookModal();
        }
    });
}

// API Calls
async function apiCall(endpoint, method = 'GET', body = null, encrypt = false) {
    const headers = {
        'Content-Type': 'application/json'
    };
    
    if (currentToken) {
        headers['Authorization'] = `Bearer ${currentToken}`;
    }
    
    const options = {
        method,
        headers
    };
    
    if (body) {
        const jsonBody = JSON.stringify(body);
        
        if (encrypt) {
            // Encrypt the payload
            const encryptedBody = await encryptString(jsonBody);
            options.body = encryptedBody;
            headers['Content-Type'] = 'text/plain'; // Encrypted data is not JSON
        } else {
            options.body = jsonBody;
        }
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, options);
        
        if (response.status === 401) {
            handleLogout();
            showNotification('Session expired. Please login again.', 'error');
            return null;
        }
        
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Request failed');
        }
        
        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            return await response.json();
        }
        
        return await response.text();
    } catch (error) {
        console.error('API Error:', error);
        showNotification(error.message, 'error');
        return null;
    }
}

// Auth Handlers
async function handleLogin(e) {
    e.preventDefault();
    
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;
    
    // Send encrypted payload
    const result = await apiCall('/api/auth/login', 'POST', { email, password }, true);
    
    if (result && result.token) {
        currentToken = result.token;
        currentUser = { userId: result.userId, email };
        
        localStorage.setItem('token', currentToken);
        localStorage.setItem('user', JSON.stringify(currentUser));
        
        showNotification('Login successful!', 'success');
        showDashboard();
    }
}

async function handleSignup(e) {
    e.preventDefault();
    
    const username = document.getElementById('signupUsername').value;
    const email = document.getElementById('signupEmail').value;
    const password = document.getElementById('signupPassword').value;
    
    // Send encrypted payload
    const result = await apiCall('/api/auth/signup', 'POST', { username, email, password }, true);
    
    if (result && result.token) {
        currentToken = result.token;
        currentUser = { userId: result.userId, email };
        
        localStorage.setItem('token', currentToken);
        localStorage.setItem('user', JSON.stringify(currentUser));
        
        showNotification('Signup successful!', 'success');
        showDashboard();
    }
}

function handleLogout() {
    currentToken = null;
    currentUser = null;
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    showLogin();
    showNotification('Logged out successfully', 'success');
}

// Book Handlers
async function loadBooks() {
    const books = await apiCall('/api/books');
    
    if (books) {
        displayBooks(books);
    }
}

function displayBooks(books) {
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
    
    const bookId = document.getElementById('bookId').value;
    const book = {
        title: document.getElementById('bookTitle').value,
        author: document.getElementById('bookAuthor').value,
        isbn: document.getElementById('bookIsbn').value,
        publishedYear: parseInt(document.getElementById('bookPublishedYear').value)
    };
    
    // Send encrypted payload
    const result = await apiCall('/api/books', 'POST', book, true);
    
    if (result) {
        showNotification('Book added successfully!', 'success');
        closeBookModal();
        loadBooks();
    }
}

async function deleteBook(id) {
    if (!confirm('Are you sure you want to delete this book?')) {
        return;
    }
    
    const result = await apiCall(`/api/books/${id}`, 'DELETE');
    
    if (result !== null) {
        showNotification('Book deleted successfully!', 'success');
        loadBooks();
    }
}

// UI Functions
function showLogin() {
    loginPage.classList.add('active');
    signupPage.classList.remove('active');
    dashboardPage.classList.remove('active');
    document.getElementById('loginForm').reset();
}

function showSignup() {
    loginPage.classList.remove('active');
    signupPage.classList.add('active');
    dashboardPage.classList.remove('active');
    document.getElementById('signupForm').reset();
}

function showDashboard() {
    loginPage.classList.remove('active');
    signupPage.classList.remove('active');
    dashboardPage.classList.add('active');
    
    if (currentUser) {
        document.getElementById('userEmail').textContent = currentUser.email;
    }
    
    loadBooks();
}

function openBookModal(book = null) {
    document.getElementById('modalTitle').textContent = book ? 'Edit Book' : 'Add Book';
    
    if (book) {
        document.getElementById('bookId').value = book.id;
        document.getElementById('bookTitle').value = book.title;
        document.getElementById('bookAuthor').value = book.author;
        document.getElementById('bookIsbn').value = book.isbn;
        document.getElementById('bookPublishedYear').value = book.publishedYear;
    } else {
        document.getElementById('bookForm').reset();
        document.getElementById('bookId').value = '';
    }
    
    bookModal.style.display = 'block';
}

function closeBookModal() {
    bookModal.style.display = 'none';
    document.getElementById('bookForm').reset();
}

function showNotification(message, type = 'info') {
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
