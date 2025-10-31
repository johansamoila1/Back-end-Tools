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
├── Controllers/
│   ├── MessagesController.cs
│   └── UsersController.cs
│
├── Models/
│   ├── Message.cs
│   ├── MessageAppContext.cs
│   ├── MessageDTO.cs
│   ├── User.cs
│   ├── UserDTO.cs
│   └── UserCreateDTO.cs
│
├── Repositories/
│   ├── IMessageRepository.cs
│   ├── IUserRepository.cs
│   ├── MessageRepository.cs
│   └── UserRepository.cs
│
├── Services/
│   ├── IMessageService.cs
│   ├── IUserService.cs
│   ├── MessageService.cs
│   └── UserService.cs
│
├── Middleware/
│   └── ApiKeyMiddleware.cs
│
├── appsettings.json
├── Program.cs
└── README.md            
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
```http
POST /api/Users

{
  "username": "omakayttaja",
  "password": "salasana123",
  "firstName": "Etunimi",
  "lastName": "Sukunimi"
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
```http
POST /api/Users/login

{
  "username": "omakayttaja",
  "password": "salasana123"
}

Vastaus:

{
  "userId": 1,
  "username": "omakayttaja"
}
```

3. Viestien lähettäminen

```http

Julkinen:

POST /api/Messages
currentUserId: 1

{
  "title": "Hei kaikille!",
  "content": "Tämä on julkinen viesti",
  "senderId": 1,
  "receiverId": null
}

Yksityinen:

POST /api/Messages
currentUserId: 1

{
  "title": "Yksityinen viesti",
  "content": "Tämä vain sinulle",
  "senderId": 1,
  "receiverId": 2
}

Vastaus:

POST /api/Messages
currentUserId: 1

{
  "title": "Aiempi viesti",
  "content": "Vastaus edelliseen viestiin",
  "senderId": 1,
  "receiverId": null,
  "previousMessageId": 1
}
```
4. Viestien lukeminen

```http
Kaikki:

GET /api/Messages/public

Omat:

GET /api/Messages/private/1
currentUserId: 1

Yksittäinen:

GET /api/Messages/1
currentUserId: 1
```
