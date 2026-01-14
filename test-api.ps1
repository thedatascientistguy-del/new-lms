# PowerShell script to test Library Management API
# Make sure the API is running before executing this script

$baseUrl = "https://localhost:7000" # Update with your actual port
$token = ""

Write-Host "=== Library Management API Test Script ===" -ForegroundColor Cyan
Write-Host ""

# Test 1: Signup
Write-Host "1. Testing Signup..." -ForegroundColor Yellow
$signupBody = @{
    username = "testuser"
    email = "test@example.com"
    password = "Test123456"
} | ConvertTo-Json

try {
    $signupResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/signup" -Method Post -Body $signupBody -ContentType "application/json" -SkipCertificateCheck
    Write-Host "Signup successful!" -ForegroundColor Green
    Write-Host "User ID: $($signupResponse.userId)"
    Write-Host "Username: $($signupResponse.username)"
    $token = $signupResponse.token
    Write-Host "Token: $token" -ForegroundColor Gray
} catch {
    Write-Host "Signup failed: $_" -ForegroundColor Red
}

Write-Host ""

# Test 2: Login
Write-Host "2. Testing Login..." -ForegroundColor Yellow
$loginBody = @{
    email = "test@example.com"
    password = "Test123456"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $loginBody -ContentType "application/json" -SkipCertificateCheck
    Write-Host "Login successful!" -ForegroundColor Green
    $token = $loginResponse.token
    Write-Host "Token updated: $token" -ForegroundColor Gray
} catch {
    Write-Host "Login failed: $_" -ForegroundColor Red
}

Write-Host ""

# Test 3: Add Book
Write-Host "3. Testing Add Book..." -ForegroundColor Yellow
$bookBody = @{
    title = "Clean Code"
    author = "Robert C. Martin"
    isbn = "978-0132350884"
    publishedYear = 2008
} | ConvertTo-Json

$headers = @{
    "Authorization" = "Bearer $token"
}

try {
    $addBookResponse = Invoke-RestMethod -Uri "$baseUrl/api/books" -Method Post -Body $bookBody -ContentType "application/json" -Headers $headers -SkipCertificateCheck
    Write-Host "Book added successfully!" -ForegroundColor Green
    Write-Host "Book ID: $($addBookResponse.id)"
    Write-Host "Title: $($addBookResponse.title)"
    $bookId = $addBookResponse.id
} catch {
    Write-Host "Add book failed: $_" -ForegroundColor Red
}

Write-Host ""

# Test 4: Get All Books
Write-Host "4. Testing Get All Books..." -ForegroundColor Yellow
try {
    $allBooks = Invoke-RestMethod -Uri "$baseUrl/api/books" -Method Get -Headers $headers -SkipCertificateCheck
    Write-Host "Retrieved $($allBooks.Count) book(s)" -ForegroundColor Green
    $allBooks | ForEach-Object {
        Write-Host "  - $($_.title) by $($_.author) (ID: $($_.id))"
    }
} catch {
    Write-Host "Get all books failed: $_" -ForegroundColor Red
}

Write-Host ""

# Test 5: Get Book by ID
if ($bookId) {
    Write-Host "5. Testing Get Book by ID..." -ForegroundColor Yellow
    try {
        $book = Invoke-RestMethod -Uri "$baseUrl/api/books/$bookId" -Method Get -Headers $headers -SkipCertificateCheck
        Write-Host "Book retrieved successfully!" -ForegroundColor Green
        Write-Host "Title: $($book.title)"
        Write-Host "Author: $($book.author)"
        Write-Host "ISBN: $($book.isbn)"
        Write-Host "Published Year: $($book.publishedYear)"
    } catch {
        Write-Host "Get book by ID failed: $_" -ForegroundColor Red
    }
    
    Write-Host ""
    
    # Test 6: Delete Book
    Write-Host "6. Testing Delete Book..." -ForegroundColor Yellow
    try {
        $deleteResponse = Invoke-RestMethod -Uri "$baseUrl/api/books/$bookId" -Method Delete -Headers $headers -SkipCertificateCheck
        Write-Host "Book deleted successfully!" -ForegroundColor Green
        Write-Host $deleteResponse.message
    } catch {
        Write-Host "Delete book failed: $_" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== Test Complete ===" -ForegroundColor Cyan
