# PowerShell script to convert markdown to PDF
# This script creates an HTML version first, then converts to PDF using built-in Windows tools

$markdownFile = "MIGRATION_ACCOMPLISHMENT.md"
$htmlFile = "MIGRATION_ACCOMPLISHMENT.html"
$pdfFile = "MIGRATION_ACCOMPLISHMENT.pdf"

# Read the markdown content
$content = Get-Content $markdownFile -Raw

# Create a simple HTML wrapper with CSS for better formatting
$html = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <title>Migration Accomplishment Report</title>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
            color: #333;
        }
        h1 { color: #2c3e50; border-bottom: 3px solid #3498db; padding-bottom: 10px; }
        h2 { color: #34495e; border-bottom: 2px solid #ecf0f1; padding-bottom: 5px; margin-top: 30px; }
        h3 { color: #7f8c8d; margin-top: 25px; }
        table {
            border-collapse: collapse;
            width: 100%;
            margin: 15px 0;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }
        th, td {
            border: 1px solid #ddd;
            padding: 12px;
            text-align: left;
        }
        th {
            background-color: #3498db;
            color: white;
            font-weight: bold;
        }
        tr:nth-child(even) {
            background-color: #f8f9fa;
        }
        code {
            background-color: #f4f4f4;
            padding: 2px 4px;
            border-radius: 3px;
            font-family: 'Consolas', monospace;
        }
        pre {
            background-color: #f4f4f4;
            padding: 15px;
            border-radius: 5px;
            overflow-x: auto;
            border-left: 4px solid #3498db;
        }
        .checkmark {
            color: #27ae60;
            font-weight: bold;
        }
        ul, ol {
            padding-left: 20px;
        }
        blockquote {
            border-left: 4px solid #3498db;
            margin-left: 0;
            padding-left: 20px;
            font-style: italic;
            color: #7f8c8d;
        }
    </style>
</head>
<body>
"@

# Convert basic markdown to HTML (simplified approach)
$htmlContent = $content -replace "^# (.*)", "&lt;h1&gt;`$1&lt;/h1&gt;"
$htmlContent = $htmlContent -replace "^## (.*)", "&lt;h2&gt;`$1&lt;/h2&gt;"
$htmlContent = $htmlContent -replace "^### (.*)", "&lt;h3&gt;`$1&lt;/h3&gt;"
$htmlContent = $htmlContent -replace "^#### (.*)", "&lt;h4&gt;`$1&lt;/h4&gt;"
$htmlContent = $htmlContent -replace "\*\*(.*?)\*\*", "&lt;strong&gt;`$1&lt;/strong&gt;"
$htmlContent = $htmlContent -replace "\*(.*?)\*", "&lt;em&gt;`$1&lt;/em&gt;"
$htmlContent = $htmlContent -replace "✅", "&lt;span class='checkmark'&gt;✅&lt;/span&gt;"
$htmlContent = $htmlContent -replace "``([^``]+)``", "&lt;code&gt;`$1&lt;/code&gt;"

# Convert line breaks to HTML
$htmlContent = $htmlContent -replace "`r?`n`r?`n", "&lt;/p&gt;&lt;p&gt;"
$htmlContent = "&lt;p&gt;$htmlContent&lt;/p&gt;"

# Add closing HTML
$html += $htmlContent + @"
</body>
</html>
"@

# Write HTML file
$html | Out-File -FilePath $htmlFile -Encoding UTF8

Write-Host "HTML file created: $htmlFile" -ForegroundColor Green

# Provide instructions for PDF conversion
Write-Host "`nTo convert to PDF, you have several options:" -ForegroundColor Yellow
Write-Host "1. Open $htmlFile in your browser and use 'Print to PDF'" -ForegroundColor Cyan
Write-Host "2. Use Microsoft Word: File > Open > $htmlFile, then Save As PDF" -ForegroundColor Cyan
Write-Host "3. Use Microsoft Edge: edge --headless --print-to-pdf=$pdfFile $htmlFile" -ForegroundColor Cyan

# Try using Microsoft Edge for PDF conversion
try {
    $edgePath = Get-Command msedge -ErrorAction Stop
    Write-Host "`nAttempting PDF conversion with Microsoft Edge..." -ForegroundColor Yellow
    Start-Process -FilePath "msedge" -ArgumentList "--headless", "--print-to-pdf=`"$pdfFile`"", "`"$htmlFile`"" -Wait
    if (Test-Path $pdfFile) {
        Write-Host "PDF successfully created: $pdfFile" -ForegroundColor Green
    }
} catch {
    Write-Host "`nMicrosoft Edge not found or PDF creation failed." -ForegroundColor Red
    Write-Host "Please use one of the manual options above." -ForegroundColor Yellow
}