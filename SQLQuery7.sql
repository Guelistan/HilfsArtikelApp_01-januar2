CREATE TABLE kunden (
    id INT identity(1,1) PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    vorname VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    passwort VARCHAR(255)
);

CREATE TABLE artikel (
    id INT identity (1,1)  PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    preis DECIMAL(5, 2) NOT NULL DEFAULT 0.5
);

CREATE TABLE warenkorb (
    id INT identity (1,1) PRIMARY KEY,
    kunden_id INT,
    artikel_id INT,
    kaufdatum DATE,
    anzahl INT,
    preis DECIMAL(5, 2),
    FOREIGN KEY (kunden_id) REFERENCES kunden(id),
    FOREIGN KEY (artikel_id) REFERENCES artikel(id)
);
