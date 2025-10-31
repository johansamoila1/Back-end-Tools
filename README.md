# Back-end harjoitustyö 

Harjoitustyö on Backend-viestintäsovellus, joka mahdollistaa julkisten ja yksityisten viestien lähettämisen, vastaamisen viesteihin sekä käyttäjähallinnan. Työ ei sisällä front-end toteutusta.

## Sovelluksen tarkoitus

Sovellus mahdollistaa:
- Käyttäjien rekisteröitymisen ja kirjautumisen
- Julkisten viestien lähettämisen (Viestit näkyvät kaikille)
- Yksityisviestien lähettämisen (Viestit näkyvät vain lähettäjälle ja vastaanottajalle)
- Vastaamisen viesteihin (viestiketjut)
- Omien viestien muokkaaminen ja poistaminen
- Oman profiilin muokkaaminen ja poistaminen

## Teknologiat

- ASP.NET Core 8.0
- Entity Framework Core
- BCrypt (salasanojen salaus)
- Swagger UI (API:n testaaminen)
- DTO (tietojen siirto)
- Middleware (API-avaimen validointi)

## Tietomallit

### User

```plaintext
- Id
- Username (pakollinen)
- Password (salattu BCrypt:llä)
- FirstName
- LastName
- JoinDate (automaattinen)
- LastLogin (päivitetään kirjautuessa)
```

### Message

```plaintext
- Id
- Title (max 100 merkkiä)
- Content (max 2000 merkkiä)
- SenderId
- ReceiverId
- PreviousMessageId
- CreatedAt (automaattinen)
```
## Projektin rakenne

```plaintext
Back_end_harjoitustyö/
│
├── Controllers/                    HTTP-pyyntöjen käsittelijät
│   ├── MessagesController.cs       - Viestien CRUD-toiminnot
│   └── UsersController.cs          - Käyttäjien hallinta ja kirjautuminen
│
├── Models/                         Tietokantamallit
│   ├── Message.cs                  - Käyttäjä ja viestimallit
│   ├── MessageAppContext.cs        - Tietokantayhteys
│   ├── MessageDTO.cs               - Tietojensiirto
│   ├── User.cs                     
│   ├── UserDTO.cs                  
│   
│
├── Repositories/                  Tietokantatoiminnot
│   ├── IMessageRepository.cs      - Tietokantarajapinnat
│   ├── IUserRepository.cs        
│   ├── MessageRepository.cs
│   └── UserRepository.cs
│
├── Services/                      Toiminnanlogiikka (oikeudet, validointi
│   ├── IMessageService.cs         - Käyttäjä ja viestipalven rajapinta
│   ├── IUserService.cs
│   ├── MessageService.cs
│   └── UserService.cs
│
├── Middleware/                    Pyyntöjen esikäsittely
│   └── ApiKeyMiddleware.cs        - API-avaimen tarkistus
│
├── appsettings.json               Tietokanta asetukset
├── Program.cs                     Sovelluksen käynnistys
└── README.md                      Sovelluksen dokumentointi
```
## API-rajapinnat

### Käyttäjät (`/api/Users`)

| Metodi | Reitti               | Kuvaus                              |
|--------|----------------------|-------------------------------------|
| `POST` | `/api/Users`         | Luo käyttäjän                      |
| `POST` | `/api/Users/login`   | Kirjautuminen  |
| `GET`  | `/api/Users`         | Hakee kaikki käyttäjät              |
| `GET`  | `/api/Users/{id}`    | Hakee oman profiilin                |
| `PUT`  | `/api/Users/{id}`    | Päivittää oman profiilin            |
| `DELETE` | `/api/Users/{id}`  | Poistaa oman tilin                  |

### Viestit (`/api/Messages`)

| Metodi | Reitti                        | Kuvaus                                      |
|--------|-------------------------------|---------------------------------------------|
| `GET`  | `/api/Messages/public`        | Hakee kaikki julkiset viestit               |
| `GET`  | `/api/Messages/private/{userId}` | Hakee yksityisviestit käyttäjälle         |
| `GET`  | `/api/Messages/{id}`          | Hakee viestin ID:llä (oma tai julkinen)     |
| `POST` | `/api/Messages`               | Lähettää viestin (julkinen tai yksityinen)  |
| `PUT`  | `/api/Messages/{id}`          | Muokkaa omaa viestiä                        |
| `DELETE` | `/api/Messages/{id}`        | Poistaa oman viestin                        |

## Käyttöohje

1. Käyttäjätunnuksen luominen
```bash

POST /api/Users

{
  "username": "omakayttaja",                 Mitä tapahtuu?
  "password": "salasana123",                 - Salasana salataan BCrypt:llä.
  "firstName": "Etunimi",                    - Luodaan uusi käyttäjä JoinDate:lla.
  "lastName": "Sukunimi"                     - Palautetaan luotu profiili (salasana piilotettu).
}

Vastaus:

{
  "id": 1,
  "username": "omakayttaja",
  "firstName": "Etunimi",
  "lastName": "Sukunimi",
  "joinDate": "automaattinen",
  "lastLogin": "automaattinen"
}
```

2. Sisäänkirjautuminen
```bash
POST /api/Users/login

{
  "username": "omakayttaja",                  Mitä tapahtuu?
  "password": "salasana123"                   - Tarkistetaan käyttäjä ja salasana.
}                                             - Päivitetään LastLogin-aika.
                                              - Palautetaan UserId ja Username.
Vastaus:

{
  "userId": 1,
  "username": "omakayttaja"
}
```

3. Viestien lähettäminen
```bash

Julkinen viesti:

POST /api/Messages
currentUserId: 1

{
  "title": "Hei kaikille!",                   Mitä tapahtuu?
  "content": "Tämä on julkinen viesti",       - Tarkistetaan, että senderId == currentUserId.
  "senderId": 1,                              - receiverId == null -> viesti on julkinen.
  "receiverId": null                          - Tallennetaan CreatedAt automaattisesti.
}

Yksityinen viesti:

POST /api/Messages
currentUserId: 1

{
  "title": "Yksityinen viesti",              Mitä tapahtuu?
  "content": "Tämä vain sinulle",            - Tarkistetaan, että vastaanottaja (ID 2) on olemassa.
  "senderId": 1,                             - Vain lähettäjä ja vastaanottaja näkevät viestin.
  "receiverId": 2
}

Vastaus aiempaan viestiin:

POST /api/Messages
currentUserId: 1

{
  "title": "Aiempi viesti",                
  "content": "Vastaus viestiin",            Mitä tapahtuu?
  "senderId": 1,                            - Tarkistetaan, että previousMessageId (1) on olemassa.
  "receiverId": null,                       - Luodaan viestiketju.
  "previousMessageId": 1
}
```
4. Viestien lukeminen
```bash

Kaikki:                                    Mitä tapahtuu?
                                           - Haetaan kaikki viestit, joissa receiverId == null
GET /api/Messages/public                   - Näkyy kaikille käyttäjille (ei kirjautumista vaadita)

Omat:

GET /api/Messages/private/1                - Tarkistetaan, että userId == currentUserId
currentUserId: 1                           - Haetaan vain viestit, joissa receiverId == 1

Yksittäinen viesti:

GET /api/Messages/1                        - Jos viesti on julkinen -> kaikki näkevät
currentUserId: 1                           - Jos yksityinen -> vain lähettäjä tai vastaanottaja.
                                           - Muuten: virhe 401.
