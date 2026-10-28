# Virtuální kasino UTB

Semestrální projekt do předmětu **Pokročilé webové technologie**.

## 👥 Členové týmu a poměr práce
| Jméno a příjmení | Role v týmu | Poměr práce |
|:---|:---|:---:|
| **Juliana Hollá** | Vedoucí | 1 |
| **Vojtěch Siuda** | Spoluvedoucí | 1 |

---

## 🚀 Spuštění projektu

1. **Požadavky:** .NET SDK (10), Visual Studio 2026 nebo JetBrains Rider.
2. **Postup:**
   - Otevřete solution `UTB_Casino.slnx` ve Visual Studiu 2026 nebo JetBrains Rider.
   - Nastavte projekt `UTB.Casino.Web` jako **Start-up projekt**.
   - Spusťte projekt (F5).
   - Při prvním spuštění proběhne automatická inicializace a seedování lokální SQLite databáze (`casino.db`) výchozími daty (vytvoření rolí, administrátorského účtu a her).

---

## 📂 Struktura řešení
Projekt využívá vrstvenou architekturu s oddělením zodpovědností:

- `UTB.Casino.Domain`: Datové entity hráčů, sázek, her a finančních transakcí.
- `UTB.Casino.Application`: Byznys logika, herní motory (Blackjack, Ruleta, Automaty) a správa zůstatků v peněžence.
- `UTB.Casino.Infrastructure`: Datová vrstva – Entity Framework Core kontext, repozitáře, transakční vzor UnitOfWork a inicializace databáze.
- `UTB.Casino.Web`: Prezenční vrstva ASP.NET Core MVC – Razor pohledy her, uživatelská rozhraní, validace věku a dedikovaná administrátorská sekce (`Areas/Admin`).
- `UTB.Casino.Tests`: Automatizované unit testy pokrývající herní logiku a chování kontrolerů.

---

## 🛠️ Klíčová implementační rozhodnutí

### 1. Autentizace a autorizace (ASP.NET Core Identity)
Ověřování uživatelů zajišťuje systém ASP.NET Core Identity integrovaný do databáze s rozdělením do rolí (`Admin`, `Customer`):
- Herní a finanční funkce jsou přístupné pouze přihlášeným a ověřeným plnoletým uživatelům (18+) pomocí custom validačního atributu pro kontrolu věku.
- Správa katalogu her, uživatelů a audit všech sázek i transakcí je izolována v administrátorské oblasti `Areas/Admin` a chráněna autorizačním atributem pro roli administrátora `[Authorize(Roles = "Admin")]`.

### 2. Herní mechaniky a klientská interaktivita
Aplikace kombinuje bezpečný výpočet na serveru s plynulou odezvou na klientské straně:
- **Ruleta a Automaty:** Veškeré vyhodnocení výsledků a sázek probíhá výhradně na backendu. Frontend pouze vizualizuje vylosovaný stav pomocí JavaScriptových animací a HTML5 Canvasu.
- **Blackjack:** Udržuje stav rozehraného kola hráče napříč jednotlivými tahy (rozdání, Hit, Stand) a dynamicky aktualizuje karty na stole.

### 3. Konzistence peněženky a atomické transakce
Správa zůstatku je striktně kontrolována v aplikační vrstvě:
- Každá sázka a výhra je evidována jako podvojný záznam: změna v tabulce sázek a odpovídající pohyb v peněžence (typy sázka, výhra, vklad).
- Všechny operace probíhají v rámci databázové transakce přes vzor `UnitOfWork`, což garantuje atomicitu a brání vzniku záporného zůstatku při rychlých či souběžných požadavcích.

---

## 📝 Poznámky k odevzdání
- **Stav:** Projekt je plně funkční (obsahuje 3 plnohodnotné hry, peněženku a administraci).
- **Testování:** Unit testy v `UTB.Casino.Tests` ověřují férovost a matematiku herních pravidel a správné reakce systému při nedostatku kreditu.
- **Problémy:** Hlavní výzvou bylo spolehlivé držení stavu rozehraného kola Blackjacku mezi jednotlivými HTTP požadavky a synchronizace vizuální animace
