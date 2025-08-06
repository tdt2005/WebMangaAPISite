CREATE TABLE Reader(
    ReaderID INT IDENTITY(1,1) PRIMARY KEY, 
    Username NVARCHAR(100),
    Email NVARCHAR(100),
    Password NVARCHAR(100)
);

CREATE TABLE Manga(
    MangaID NVARCHAR(100) PRIMARY KEY,
    Title NVARCHAR(MAX), 
    Genres NVARCHAR(200),
    Thumbnails NVARCHAR(MAX),
    Descriptions NVARCHAR(MAX)
);

CREATE TABLE Chapter (
    ChapterID NVARCHAR(100) PRIMARY KEY,
    MangaID NVARCHAR(100),
    chapter_no FLOAT,
    CONSTRAINT FK_Follow_Manga FOREIGN KEY (MangaID) REFERENCES Manga(MangaID)
);

CREATE TABLE Content (
    ContentID INT IDENTITY(1,1) PRIMARY KEY, -- Auto-incrementing integer
    ChapterID NVARCHAR(100),
    Image_no INT,
    Image_path NVARCHAR(MAX) NOT NULL,
    CONSTRAINT FK_Follow_Chapter FOREIGN KEY (ChapterID) REFERENCES Chapter(ChapterID)
);

CREATE TABLE Follow(
    ReaderID INT,  -- Changed from NVARCHAR(100) to INT
    MangaID NVARCHAR(100),
    CONSTRAINT PK_Follow PRIMARY KEY (ReaderID, MangaID),
    CONSTRAINT FK_Follow_Reader FOREIGN KEY (ReaderID) REFERENCES Reader(ReaderID),
    CONSTRAINT FK_Follow_Manga1 FOREIGN KEY (MangaID) REFERENCES Manga(MangaID)
);
CREATE TABLE Comment (
    CommentID INT IDENTITY(1,1) PRIMARY KEY,      
    ReaderID INT NOT NULL,                          
    MangaID NVARCHAR(100) NOT NULL,
    ChapterID NVARCHAR(100) NULL,
    CommentText NVARCHAR(MAX) NOT NULL,
    CommentDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Comment_Reader FOREIGN KEY (ReaderID) REFERENCES Reader(ReaderID),
    CONSTRAINT FK_Comment_Manga FOREIGN KEY (MangaID) REFERENCES Manga(MangaID),
    CONSTRAINT FK_Comment_Chapter FOREIGN KEY (ChapterID) REFERENCES Chapter(ChapterID)
);
