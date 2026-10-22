# Virtuální kasino UTB

Semestrální projekt do předmětu **Pokročilé webové technologie**.

## 👥 Členové týmu a poměr práce
| Jméno a příjmení | Role v týmu | Poměr práce |
|:---|:---|:---:|
| **Juliana Hollá** | Vedoucí | 1 |
| **Vojěch Siuda** | Poddaný | 1 |


---

## 🚀 Spuštění projektu

1. **Požadavky:** .NET 10 SDK, Docker Desktop nebo Podman (nutný pro běh databázového Serveru a Keycloaku v Aspire).
2. **Postup:**
   - Spusťe Docker Desktop nebo Podman.
   - Otevřete solution `UTB_Casino` ve Visual Studiu 2026 nebo JetBrains Rider.
   - Nastavte projekt `UTB_Casino.AppHost` jako **Start-up projekt**.
   - Spusťte projekt.

---

## 📂 Struktura řešení
Pokud se struktura liší (například CanteenClient je rozdělený na dva projekty) tak zde uveďtte.

- `UTB.Minute.AppHost`: Aspire orchestrace.
- `UTB.Minute.Db`: Datové entity a `DbContext`.
- `UTB.Minute.DbManager`: Obsahuje endpoint pro **Http Command** (reset databáze).
- `UTB.Minute.Contracts`: Sdílená DTO, aby byla zajištěna typová bezpečnost mezi API a klienty.
- `UTB.Minute.WebApi`: Hlavní byznys logika, správa objednávek a SSE hub.
- `UTB.Minute.AdminClient`: Aplikace pro vedení menzy (správa jídel a menu).
- `UTB.Minute.CanteenClient`: Společné rozhraní pro studenty a kuchařky (ošetřeno autorizací).

## 🛠️ Klíčová implementační rozhodnutí

### 1. Autorizace a Keycloak
Zde popište, jak jste nastavili Keycloak (např. názvy rolí) a jak WebAPI ověřuje tokeny (např. pomocí `[Authorize(Roles = "...")]`).

### 2. Real-time notifikace (SSE)
Popište, jakým způsobem WebAPI distribuuje zprávy o změně stavu objednávky a jak na to reaguje Blazor komponenta (např. automatické překreslení seznamu).

### 3. Business pravidla
Jakým způsobem ošetřujete počet porcí v menu? (Např. atomická operace při objednání, aby nedošlo k "přeobjednání" při více požadavcích naráz).

---

## 📝 Poznámky k odevzdání
* **Stav:** Projekt je plně funkční.
* **Testování:** Unit testy v `UTB.Minute.WebAPI.Tests` pokrývají scénář od vytvoření jídla až po jeho výdej.
* **Problémy:** Zde uveďte, na čem jste se nejvíce zasekli a jak jste to vyřešili.

---

## 🧪 Seznam API endpointů (ukázka)
* `GET /api/meals` - Seznam všech jídel.
* `POST /api/orders` - Vytvoření nové objednávky (student).
* `GET /api/orders/sse` - Stream pro real-time notifikace.
