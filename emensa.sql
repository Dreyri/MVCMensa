-- DROP DATABASE IF EXISTS EMensa;
-- CREATE DATABASE IF NOT EXISTS EMensa;
-- USE EMensa;

-- Tabellen

DROP TABLE IF EXISTS MahlzeitenDeklarationen;
DROP TABLE IF EXISTS FHAngehoerigerFachbereich;
DROP TABLE IF EXISTS BenutzerBefreundetMit;
DROP TABLE IF EXISTS MahlzeitenBestellungen;
DROP TABLE IF EXISTS MahlzeitenBilder;
DROP TABLE IF EXISTS MahlzeitenZutat;
DROP TABLE IF EXISTS Kommentare;
DROP TABLE IF EXISTS Fachbereiche;
DROP TABLE IF EXISTS Studenten;
DROP TABLE IF EXISTS Mitarbeiter;
DROP TABLE IF EXISTS FHAngehoerige;
DROP TABLE IF EXISTS Gaeste;
DROP TABLE IF EXISTS Deklarationen;
DROP TABLE IF EXISTS Mahlzeiten;
DROP TABLE IF EXISTS Zutaten;
DROP TABLE IF EXISTS Kategorien;
DROP TABLE IF EXISTS Bilder;
DROP TABLE IF EXISTS Preise;
DROP TABLE IF EXISTS Bestellungen;
DROP VIEW IF EXISTS v_Rolle;

DROP TABLE IF EXISTS Benutzer;
CREATE TABLE Benutzer (
	Geburtsdatum DATE,					-- optional, daher kein NOT NULL benötigt
	Vorname VARCHAR(100) NOT NULL,		-- Name unterteilt in Vor- und Nachname		
	Nachname VARCHAR(50) NOT NULL,
	Aktiv BOOLEAN DEFAULT FALSE NOT NULL, -- BOOLEAN is a TINYINT(1)
	CHECK(Aktiv IN (TRUE, FALSE)),
	Anlegedatum DATE DEFAULT CURRENT_DATE NOT NULL,
	Salt VARCHAR(32),
	Hashk VARCHAR(24),
	Nutzername VARCHAR(100) UNIQUE NOT NULL,	
	Nummer INT UNSIGNED PRIMARY KEY AUTO_INCREMENT NOT NULL,
	LetzterLogin DATE DEFAULT NULL,
	EMail VARCHAR(100) UNIQUE NOT NULL
)ENGINE=INNODB;


CREATE TABLE Bestellungen (
	AbholZeitpunkt DATETIME NOT NULL,
	BestellZeitpunkt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	CHECK(BestellZeitpunkt < AbholZeitpunkt),
	Nummer INT UNSIGNED PRIMARY KEY AUTO_INCREMENT NOT NULL,
	Endpreis DECIMAL,
	BenutzerID INT UNSIGNED NOT NULL,
	CONSTRAINT FK_BestellungID FOREIGN KEY (BenutzerID) REFERENCES Benutzer (Nummer)
)ENGINE=INNODB;


CREATE TABLE Preise (
	ID INT UNSIGNED AUTO_INCREMENT NOT NULL,
	Jahr INT UNSIGNED NOT NULL DEFAULT YEAR(CURDATE()), 		-- ToDo: gestrichelt unterstrichen?
	Gastpreis DECIMAL(4,2) NOT NULL,
	CHECK(Gastpreis < 100),
	Studentenpreis DECIMAL(4,2),
	MAPreis DECIMAL(4,2),
	CHECK(Studentenpreis < MAPreis),
	PRIMARY KEY (ID, Jahr)
)ENGINE=INNODB;


CREATE TABLE Bilder (
	ID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT NOT NULL,
	AltText VARCHAR(100) NOT NULL,
	Titel VARCHAR(100),
	Binaerdaten VARCHAR(100) NOT NULL
)ENGINE=INNODB;


CREATE TABLE Kategorien (
	Bezeichnung VARCHAR(100) NOT NULL,
	ID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT NOT NULL,
	Oberkategorie INT UNSIGNED DEFAULT NULL,
	CONSTRAINT OberKat FOREIGN KEY (Oberkategorie) REFERENCES Kategorien (ID),
	BildID INT UNSIGNED,
	CONSTRAINT FK_BildID FOREIGN KEY (BildID) REFERENCES Bilder (ID),
	PreisID INT UNSIGNED NOT NULL,
	CONSTRAINT FK_Preis FOREIGN KEY (PreisID) REFERENCES Preise (ID)
)ENGINE=INNODB;


CREATE TABLE Zutaten (
	ID MEDIUMINT(5) PRIMARY KEY NOT NULL DEFAULT 10000,
	CHECK(ID > 9999 && ID < 100000),
	Name VARCHAR(100) NOT NULL,
	Bio BOOLEAN DEFAULT FALSE NOT NULL,
	CHECK(Bio IN (TRUE, FALSE)),
	Vegetarisch BOOLEAN DEFAULT FALSE NOT NULL,
	CHECK(Vegetarisch IN (TRUE, FALSE)),
	Vegan BOOLEAN DEFAULT FALSE NOT NULL,
	CHECK(Vegan IN (TRUE, FALSE)),
	Glutenfrei BOOLEAN DEFAULT FALSE NOT NULL,
	CHECK(Glutenfrei IN (TRUE, FALSE))
)ENGINE=INNODB;


CREATE TABLE Mahlzeiten (
	Vorrat INT UNSIGNED NOT NULL DEFAULT 0,
	Verfuegbar BOOLEAN AS (Vorrat <> 0),
	ID INT UNSIGNED PRIMARY KEY NOT NULL AUTO_INCREMENT,
	KategorieID INT UNSIGNED NOT NULL,
	PreisID INT UNSIGNED NOT NULL,
	Beschreibung VARCHAR(140) NOT NULL DEFAULT "Keine Beschreibung verfuegbar",
	#BildID INT UNSIGNED NOT NULL,
	#ZutatID MEDIUMINT(5),
	CONSTRAINT FK_PreisID FOREIGN KEY (PreisID) REFERENCES Preise (ID),
	CONSTRAINT FK_KategorieID FOREIGN KEY (KategorieID) REFERENCES Kategorien (ID)
	#CONSTRAINT FK_Bild1ID FOREIGN KEY (BildID) REFERENCES Bilder (ID),
	#CONSTRAINT FK_ZutatID FOREIGN KEY (ZutatID) REFERENCES Zutaten (ID)
)ENGINE=INNODB;


CREATE TABLE Deklarationen (
	Zeichen VARCHAR(2) PRIMARY KEY NOT NULL,
	Beschriftung VARCHAR(64) NOT NULL
)ENGINE=INNODB;

CREATE TABLE Gaeste (
	Nummer INT UNSIGNED NOT NULL, 
	Grund VARCHAR(255) NOT NULL,
	Ablaufdatum DATE DEFAULT DATE_ADD(CURRENT_DATE, INTERVAL 1 WEEK),
	PRIMARY KEY (Nummer),
	CONSTRAINT FK_GaesteID FOREIGN KEY (Nummer) REFERENCES Benutzer(Nummer) ON DELETE CASCADE
)ENGINE=INNODB;


CREATE TABLE FHAngehoerige (
	Nummer INT UNSIGNED NOT NULL,
	PRIMARY KEY (Nummer),
	CONSTRAINT FK_FHAngehoerigeID FOREIGN KEY (Nummer) REFERENCES Benutzer(Nummer) ON DELETE CASCADE
)ENGINE=INNODB;


CREATE TABLE Mitarbeiter (
	Nummer INT UNSIGNED NOT NULL,
	Buero VARCHAR(25),
	Telefon VARCHAR(25),
	PRIMARY KEY (Nummer),
	CONSTRAINT FK_MitarbeiterID FOREIGN KEY(Nummer) REFERENCES FHAngehoerige (Nummer) ON DELETE CASCADE
)ENGINE=INNODB;


CREATE TABLE Studenten (
	Nummer INT UNSIGNED NOT NULL,
	Studiengang VARCHAR(3) NOT NULL,
	CHECK (Studiengang IN ('ET', 'INF', 'ISE', 'MCD', 'WI')),
	Matrikelnummer INT NOT NULL UNIQUE,
	CHECK(Matrikelnummer > 9999999 && Matrikelnummer < 1000000000),
	PRIMARY KEY (Nummer),
	CONSTRAINT FK_StudentenID FOREIGN KEY(Nummer) REFERENCES FHAngehoerige (Nummer) ON DELETE CASCADE
)ENGINE=INNODB;


CREATE TABLE Fachbereiche (
	ID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT NOT NULL,
	Name VARCHAR(100) NOT NULL,
	Website VARCHAR(100) NOT NULL
)ENGINE=INNODB;


CREATE TABLE Kommentare (
	ID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT NOT NULL,
	Bemerkung VARCHAR(255),
	Bewertung INT UNSIGNED DEFAULT 0 NOT NULL,
	MahlzeitID INT UNSIGNED NOT NULL,
	CONSTRAINT FK_MahlzeitIDKommentare FOREIGN KEY (MahlzeitID) REFERENCES Mahlzeiten(ID),
	StudentID INT UNSIGNED NOT NULL,
	CONSTRAINT FK_Student FOREIGN KEY (StudentID) REFERENCES Studenten (Nummer)
)ENGINE=INNODB;

-- Relationstabellen


CREATE TABLE MahlzeitenZutat (
  MahlzeitID INT UNSIGNED NOT NULL,
  ZutatID MEDIUMINT(5) NOT NULL,
  PRIMARY KEY (MahlzeitID,ZutatID),
  KEY FK_Zutat (ZutatID),
  CONSTRAINT FK_MahlzeitIDZutat FOREIGN KEY (MahlzeitID) REFERENCES Mahlzeiten (ID),
  CONSTRAINT FK_Zutat FOREIGN KEY (ZutatID) REFERENCES Zutaten (ID)
)ENGINE=INNODB;




CREATE TABLE MahlzeitenBilder (
	MahlzeitID INT UNSIGNED NOT NULL,
	BildID INT UNSIGNED NOT NULL,
	PRIMARY KEY (MahlzeitID, BildID),
	KEY FK_Bild (BildID),
	CONSTRAINT FK_MahlzeitBilder FOREIGN KEY (MahlzeitID) REFERENCES Mahlzeiten (ID),
	CONSTRAINT FK_Bild FOREIGN KEY (BildID) References Bilder (ID)
)ENGINE=INNODB;




CREATE TABLE MahlzeitenBestellungen (
	MahlzeitID INT UNSIGNED NOT NULL,
	BestellungID INT UNSIGNED NOT NULL,
	Anzahl INT UNSIGNED NOT NULL,
	PRIMARY KEY (MahlzeitID, BestellungID),
	KEY FK_Bestellung (BestellungID),
	CONSTRAINT FK_MahlzeitIDBestellung FOREIGN KEY (MahlzeitID) REFERENCES Mahlzeiten (ID),
	CONSTRAINT FK_Bestellung FOREIGN KEY (BestellungID) References Bestellungen (Nummer)
)ENGINE=INNODB;



CREATE TABLE BenutzerBefreundetMit (
	Benutzer1ID INT UNSIGNED NOT NULL,
	Benutzer2ID INT UNSIGNED NOT NULL,
	PRIMARY KEY (Benutzer1ID, Benutzer2ID),
	KEY FK_Benutzer1ID (Benutzer1ID),
	CONSTRAINT FK_Benutzer1ID FOREIGN KEY (Benutzer1ID) REFERENCES Benutzer (Nummer),
	CONSTRAINT FK_Benutzer2ID FOREIGN KEY (Benutzer2ID) REFERENCES Benutzer (Nummer)
)ENGINE=INNODB;




CREATE TABLE FHAngehoerigerFachbereich (
	FHAngehoeriger INT UNSIGNED NOT NULL,
	Fachbereich INT UNSIGNED NOT NULL,
	PRIMARY KEY (FHAngehoeriger, Fachbereich),
	KEY FK_Benutzer3ID (FHAngehoeriger),
	CONSTRAINT FK_Benutzer3ID FOREIGN KEY (FHAngehoeriger) REFERENCES Benutzer (Nummer),
	CONSTRAINT FK_FachbereichID FOREIGN KEY (Fachbereich) REFERENCES FHAngehoerige (Nummer)
)ENGINE=INNODB;




CREATE TABLE MahlzeitenDeklarationen (
	MahlzeitID INT UNSIGNED NOT NULL,
	DeklarationZeichen VARCHAR(2) NOT NULL,
	PRIMARY KEY (MahlzeitID, DeklarationZeichen),
	KEY FK_MahlzeitID (MahlzeitID),
	CONSTRAINT FK_MahlzeitID FOREIGN KEY (MahlzeitID) REFERENCES Mahlzeiten (ID),
	CONSTRAINT FK_DeklarationZeichen FOREIGN KEY (DeklarationZeichen) REFERENCES Deklarationen (Zeichen)
)ENGINE=INNODB;

ALTER TABLE Fachbereiche ADD Adresse VARCHAR(128);
ALTER TABLE Mahlzeiten ADD Name VARCHAR(25);

CREATE VIEW v_Rolle AS
SELECT benutzer.Nummer, benutzer.Nutzername, benutzer.Hashk, benutzer.Salt, gaeste.Nummer AS GastNum, studenten.Nummer AS StudentNum, mitarbeiter.Nummer as MANum, 
(IF(gaeste.Nummer IS NOT NULL, "Gast", IF(studenten.Nummer IS NOT NULL, "Student", IF(mitarbeiter.Nummer IS NOT NULL, "Mitarbeiter", "")))) AS Rolle FROM benutzer
LEFT JOIN studenten on studenten.Nummer = benutzer.Nummer
LEFT JOIN mitarbeiter ON mitarbeiter.Nummer = benutzer.Nummer
LEFT JOIN gaeste ON gaeste.Nummer = benutzer.Nummer;

-- insert u generic users

INSERT INTO Benutzer (Geburtsdatum, Vorname, Nachname, Aktiv, Nutzername, EMail)
VALUES
(CURRENT_DATE, 'Nils', 'Kochendörfer', TRUE, 'Pilsesucher0', 'pilsesucher0@yahoo.de'),
(CURRENT_DATE, 'Nils', 'Kochendörfer', TRUE, 'Pilsesucher1', 'pilsesucher1@yahoo.de'),
(CURRENT_DATE, 'Nils', 'Kochendörfer', TRUE, 'Pilsesucher2', 'pilsesucher2@yahoo.de'),
(CURRENT_DATE, 'Nils', 'Kochendörfer', TRUE, 'Pilsesucher3', 'pilsesucher3@yahoo.de');

-- specialise generic users as `FHAngehörige`

INSERT INTO FHAngehoerige (Nummer)
VALUES
(1),
(2),
(3),
(4);

INSERT INTO Mitarbeiter (Nummer, Buero, Telefon)
VALUES
(1, 'E126', '015159875684');

INSERT INTO Studenten (Nummer, Studiengang, Matrikelnummer)
VALUES
(2, 'INF', 999999999),
(3, 'MCD', 888888888);

-- show that cascade works

DELETE FROM Benutzer WHERE Nummer = 2;
DELETE FROM Benutzer WHERE Nummer = 1;

-- insert images

INSERT INTO Bilder (AltText, Titel, Binaerdaten)
VALUES
('Bratrolle', 'Bratrolle', '/img/produkte/Bratrolle.jpg'),
('Curry Wok', 'Curry Wok', '/img/produkte/CurryWok.jpg'),
('Currywurst', 'Currywurst', '/img/produkte/CurryWurst.jpg'),
('Falafel', 'Falafel', '/img/produkte/Falafel.png'),
('Kaesebrot', 'Kaesebrot', '/img/produkte/Kaesebrot.jpg'),
('Krautsalat', 'Krautsalat', '/img/produkte/KrautSalat.jpg'),
('Schnitzel', 'Schnitzel', '/img/produkte/Schnitzel.jpg'),
('Spiegelei', 'Spiegelei', '/img/produkte/Spiegelei.jpg');

-- insert prices

INSERT INTO Preise(Gastpreis, Studentenpreis, MAPreis)
VALUES
(3.20, 2.80, 3.00),
(3.00, 2.10, 2.60);

-- insert kategorie

INSERT INTO Kategorien (Bezeichnung, Oberkategorie, PreisID)

VALUES
('Um die Welt', NULL, 1),
('Saisonal', NULL, 1),
('Italienisches', 1, 2),
('Amerikanisches', 1, 1),
('Ungarisches', 1, 1),
('Schwedisches', 1, 1),
('Griechisches', 1, 1),
('Mexikanisches', 1, 1),
('Winter', 2, 1);

INSERT INTO Zutaten (ID, Name, Bio, Vegetarisch, Vegan, Glutenfrei)
VALUES
(10001, 'Ei', TRUE, TRUE, FALSE, TRUE),
(10002, 'Kichererbsen', TRUE, TRUE, TRUE, TRUE),
(10003, 'Fleisch', FALSE, FALSE, FALSE, TRUE);

INSERT INTO Mahlzeiten (Vorrat, Name, KategorieID, PreisID)
VALUES
(9, 'Bratrolle', 5, 1),
(8, 'CurryWok', 6, 1),
(7, 'CurryWurst', 7, 1),
(6, 'Falafel', 8, 1),
(0, 'KaeseBrot', 9, 2),
(0, 'Krautsalat', 5, 2),
(0, 'Schnitzel', 5, 2),
(0, 'Spiegelei', 5, 2);

INSERT INTO mahlzeitenzutat (MahlzeitID, ZutatID)
VALUES
(1, 10003),
(2, 10001),
(3, 10003),
(4, 10003),
(5, 10002),
(6, 10001),
(7, 10003),
(8, 10001);

INSERT INTO mahlzeitenbilder (MahlzeitID, BildID)
VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5),
(6, 6),
(7, 7),
(8, 8);


REPLACE INTO benutzer (`Nummer`, `Vorname`, `Nachname`, `EMail`, `Nutzername`, `LetzterLogin`, `Anlegedatum`, `Geburtsdatum`, `Salt`, `Hashk`, `Aktiv`) VALUES (21, 'Bugs', 'Findmore', 'dbwt2018@ismypassword.com', 'bugfin', '2018-11-14 17:44:10', '2018-11-14', '1996-12-13', 'MPVdLDf0zNVzpOHP+GmRxoBg9mdJIlc5', '4nx5U6DIE+N8xsbpwUr3Q1KG', 1);
REPLACE INTO benutzer (`Nummer`, `Vorname`, `Nachname`, `EMail`, `Nutzername`, `LetzterLogin`, `Anlegedatum`, `Geburtsdatum`, `Salt`, `Hashk`, `Aktiv`) VALUES (22, 'Donald', 'Truck', 'testing@ismypassword.com', 'dot', '2018-11-14 17:44:10', '2018-11-14', '1991-12-11', 'Ydn1iGl08JvvkVExSEiKDQhfYOaCtgOO', 'm5kZ68YVNU3xBiDqorthK9UP', 1);
REPLACE INTO benutzer (`Nummer`, `Vorname`, `Nachname`, `EMail`, `Nutzername`, `LetzterLogin`, `Anlegedatum`, `Geburtsdatum`, `Salt`, `Hashk`, `Aktiv`) VALUES (23, 'Fiona', 'Index', 'an0ther@ismypassword.com', 'fionad', '2018-11-14 17:44:10', '2018-11-14', '1993-12-10', 'I5GXy7BwYU2t3pHZ5YkBfKMbvN7Sr81O', 'oYylNvPe7YmjO1IHNdLA/XxJ', 1);
REPLACE INTO benutzer (`Nummer`, `Vorname`, `Nachname`, `EMail`, `Nutzername`, `LetzterLogin`, `Anlegedatum`, `Geburtsdatum`, `Salt`, `Hashk`, `Aktiv`) VALUES (24, 'Wendy', 'Burger', 's3cr3tz@ismypassword.com', 'bkahuna', '2018-11-14 17:44:10', '2018-11-14', '1982-12-12', 't1TAVguVwIiejXf3baaObIAtPx7Y+2iY', 'IMK2n5r8RUVFo4bMMS8uDyH4', 1);
REPLACE INTO benutzer (`Nummer`, `Vorname`, `Nachname`, `EMail`, `Nutzername`, `LetzterLogin`, `Anlegedatum`, `Geburtsdatum`, `Salt`, `Hashk`, `Aktiv`) VALUES (25, 'Monster', 'Robot', '^;_`;^@ismypassword.com', 'root', '2018-11-14 17:44:10', '2018-11-14', '1982-12-12', 'dX8YsBM9atpYto9caWHJM6Eet7bUngxk', 'nRt3MSBdNUHPj/q02WPgXaDA', 1);

INSERT INTO fhangehoerige (Nummer) VALUES
(21),
(22);
INSERT INTO studenten (Nummer, Studiengang, Matrikelnummer) VALUES
(21, "Inf", 31000000);

INSERT INTO mitarbeiter (Nummer, Buero, Telefon) VALUES
(22, "Hier", "Nicht");

INSERT INTO gaeste (Nummer, Grund) VALUES
(23, "Test"),
(24, "Test"),
(25, "Test");

DROP TRIGGER IF EXISTS OnOrderDo;
DELIMITER //
CREATE TRIGGER OnOrderDo
	AFTER INSERT
	ON `mahlzeitenbestellungen`
	FOR EACH ROW
	BEGIN
		UPDATE mahlzeiten
		SET Vorrat = Vorrat - NEW.Anzahl
		WHERE ID = NEW.MahlzeitID;
	END//