# Minutnik Pomodoro — Avalonia .NET

Aplikacja minutnika Pomodoro zrealizowana w technologii Avalonia UI (.NET 10) zgodnie ze wszystkimi zasadami projektowymi, wytycznymi z lekcji oraz blokiem „Własną ręką: lista”.

---

## 1. Cztery liczby z tej lekcji

* **Data pomiaru:** 20 października 2026 r.
* **Narzędzie do tokenizacji:** `Microsoft.ML.Tokenizers` / `tiktoken` (tokenizery `o200k_base` dla modeli GPT-4o oraz `cl100k_base` dla GPT-4)
* **Model asystenta:** Google Gemini (`Gemini 3.8 Flash`)

| Parametr | Wartość | Źródło / Link |
| :--- | :--- | :--- |
| **1. Liczba tokenów pierwszej wiadomości (po polsku)** | **155 tokenów** (`o200k_base` / GPT-4o)<br>*(170 tokenów w `cl100k_base` / GPT-4)* | Obliczono narzędziem tokenizacji dla oryginalnego promptu (73 słowa, 545 znaków) |
| **2. Liczba tokenów pierwszej wiadomości (po angielsku)** | **109 tokenów** (`o200k_base` / GPT-4o)<br>*(108 tokenów w `cl100k_base` / GPT-4)* | Obliczono narzędziem tokenizacji dla angielskiego odpowiednika promptu (68 słów, 469 znaków) |
| **3. Okno kontekstu modelu (Context Window)** | **1 048 576 tokenów** (1M tokenów) | [Oficjalna dokumentacja Google Gemini API](https://ai.google.dev/gemini-api/docs/models/gemini)<br>[Karta modelu Gemini 3.8 Flash](https://deepmind.google/models/model-cards/gemini-3-8-flash/) |
| **4. Maksymalna długość odpowiedzi (Max Output Tokens)** | **65 536 tokenów** (domyślny limit w wybranych środowiskach: 8 192 tokeny) | [Oficjalna dokumentacja Google Gemini API](https://ai.google.dev/gemini-api/docs/models/gemini)<br>[Dokumentacja limitów tokenów](https://ai.google.dev/gemini-api/docs/tokens) |

> **Wniosek dotyczący tokenizacji:**  
> Ta sama prośba zapisana w języku polskim zajmuje o **42%–57% więcej tokenów** niż w języku angielskim (155 vs 109 tokenów w tokenizerze `o200k_base`). Wynika to ze specyfiki algorytmu Byte-Pair Encoding (BPE), którego słowniki bazują głównie na języku angielskim. Polskie znaki diakrytyczne (ą, ć, ę, ł, ń, ó, ś, ź, ż) oraz bogata odmiana fleksyjna powodują dzielenie pojedynczych wyrazów na mniejsze pod-tokeny.

---

## 2. Wynik próby wpisania wartości `25,5` oraz `25.5`

Badanie zachowania kontrolki `NumericUpDown` w środowisku z polskimi ustawieniami regionalnymi (`CultureInfo: pl-PL`):

1. **Wpisanie wartości `25,5` (z przecinkiem):**
   * **Wynik:** Wartość zostaje **zaakceptowana i poprawnie sparsowana** (`Success: True`, pole przyjmuje wartość `25,5`).
   * **Przyczyna:** W polskiej kulturze (`pl-PL`) oficjalnym separatorem dziesiętnym jest przecinek (`NumberDecimalSeparator = ','`). Kontrolka Avalonia `NumericUpDown` poprawnie rozpoznaje wprowadzony ułamek jako 25 i pół minuty.

2. **Wpisanie wartości `25.5` (z kropką):**
   * **Wynik:** Wartość zostaje **odrzucona** (`Success: False`), a kontrolka cofa wpis (revert) do poprzedniej poprawnej wartości (`25`).
   * **Przyczyna:** Kropka `.` nie jest separatorem dziesiętnym w kulturze `pl-PL`. Podczas próby zatwierdzenia tekstu parser Avalonia (`ConvertTextToValueCore`) zgłasza błąd formatu danych (`System.IO.InvalidDataException: Input string was not in a correct format`), w wyniku czego kontrolka odrzuca błędny wpis i przywraca stan sprzed edycji.

---

## 3. Realizacja 7 zasad minutnika

Wszystkie siedem zasad zostało spełnionych jednocześnie w aplikacji:

1. **Odmierzanie 25 minut pracy, potem 5 minut przerwy i liczenie cykli**: Podstawowy cykl odlicza 25 minut pracy, po czym przechodzi do 5 minut przerwy i zlicza ukończone cykle.
2. **Zmiana obu długości w oknie**: Zastosowano kontrolki `NumericUpDown` dla czasu pracy (`WorkDurationInput`) i czasu przerwy (`BreakDurationInput`).
3. **Jeden przycisk start/pauza, drugi reset**: Przycisk `StartPauseButton` przełącza stan odliczania (Start / Pauza), a przycisk `ResetButton` przywraca minutnik do stanu wyjściowego.
4. **Czas widoczny jako mm:ss, licznik cykli widoczny obok**: Czas formatowany jest jako dwucyfrowe minuty i sekundy (`TimerText`), a licznik ukończonych cykli (`CycleCountText`) znajduje się bezpośrednio obok.
5. **Automatyczny start przerwy po pracy**: Po upłynięciu czasu pracy minutnik samoczynnie przełącza się na przerwę i kontynuuje odliczanie bez konieczności klikania przycisku.
6. **Interfejs po polsku, nazwy w kodzie po angielsku, bez dodatkowych bibliotek**: Wszystkie teksty widoczne dla użytkownika są w języku polskim, identyfikatory w kodzie C# i XAML w języku angielskim, użyto wyłącznie standardowej biblioteki Avalonia UI.
7. **Cała logika wprost w pliku MainWindow.axaml.cs**: Brak zewnętrznych klas, serwisów pośredniczących czy osobnych ViewModeli — pełna obsługa znajduje się w pliku `MainWindow.axaml.cs`.

---

## 4. Blok „Własną ręką: lista” — Historia skończonych cykli

W oknie aplikacji umieszczono listę historii ukończonych cykli (`CycleHistoryListBox`):
* Po każdym skończonym cyklu (zakończeniu przerwy) do listy dodawany jest nowy wpis z numerem cyklu, długością pracy i przerwy oraz godziną zakończenia.
* Zmiana została oznaczona osobnym dedykowanym commitem o treści `wlasna reka: historia cykli`.
