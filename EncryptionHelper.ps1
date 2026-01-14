# PowerShell Encryption Helper for Testing
# This script helps encrypt payloads for testing the API

param(
    [Parameter(Mandatory=$true)]
    [string]$PlainText
)

# Note: This is a simplified version for testing
# The actual encryption happens in the .NET application using OS-based keys

function Encrypt-String {
    param([string]$text)
    
    # For testing purposes, we'll use Base64 encoding
    # In production, the API handles proper AES encryption
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($text)
    return [Convert]::ToBase64String($bytes)
}

Write-Host "Original Text:" -ForegroundColor Yellow
Write-Host $PlainText
Write-Host ""
Write-Host "Encrypted (Base64 for testing):" -ForegroundColor Green
$encrypted = Encrypt-String -text $PlainText
Write-Host $encrypted
Write-Host ""
Write-Host "Note: The API uses OS-based AES encryption. This is just for demonstration." -ForegroundColor Cyan
