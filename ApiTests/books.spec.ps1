Describe "Books API Tests" {
    
    BeforeAll {
        $baseUrl = "http://localhost:5082/books"
        $body = @{
            Title = "Test Book"
            Author = "Test Author"
            YearPublished = 2026
        }

        $bodyUpdate = @{
            Title = "Updated Test Book"
            Author = "Updated Test Author"
            YearPublished = 2020
        }

        $bookid = 1
    }

    It "Should create a new book" {
        $response = Invoke-WebRequest -Uri $baseUrl -Method Post -Body ($body | ConvertTo-Json) -ContentType "application/json"
        $response.statusCode | Should -Be 201
    }

    It "Should retrieve all books" {
        $response = Invoke-WebRequest -Uri $baseUrl -Method Get -ContentType "application/json"
        $response.statusCode | Should -Be 200
        $response.Content | Should -Not -BeNullOrEmpty
    }

    It "Should retrieve a book by ID" {
        Write-Host "$baseUrl/$bookid"
        $response = Invoke-WebRequest -Uri "$baseUrl/$bookid" -Method Get -ContentType "application/json"
        $response.statusCode | Should -Be 200
        $response.Content | Should -Not -BeNullOrEmpty
        $book = $response.Content | ConvertFrom-Json
        $book.title | Should -Be $body.Title
        $book.author | Should -Be $body.Author
        $book.yearPublished | Should -Be $body.YearPublished
    }

    It "Should update a book by ID" {
        $response = Invoke-WebRequest -Uri "$baseUrl/$bookid" -Method Put -ContentType "application/json" -Body ($bodyUpdate | ConvertTo-Json)
        $response.statusCode | Should -Be 201
        $responseUpdatedBook = $(Invoke-WebRequest -Uri "$baseUrl/$bookid" -Method Get -ContentType "application/json").Content | ConvertFrom-Json
        $responseUpdatedBook.title | Should -Be $bodyUpdate.Title
        $responseUpdatedBook.author | Should -Be $bodyUpdate.Author
        $responseUpdatedBook.yearPublished | Should -Be $bodyUpdate.YearPublished
    }

    It "Delete book" {
        $response = Invoke-WebRequest -Uri "$baseUrl/$bookid" -Method Delete -ContentType "application/json"
        $response.statusCode | Should -Be 204
    }


}