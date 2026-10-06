namespace Serenity.Services.SqlErrors;

public partial class DefaultSqlErrorExtractor
{
    /// <summary>
    /// Regexes generated from SQL Server sys.messages for the handled constraint
    /// violation message ids (515, 547, 2601, 2627), one per language. Named groups:
    /// <c>table</c>, <c>column</c>, <c>constraint</c>, <c>value</c>.
    /// </summary>
    protected static readonly Regex[] SqlServerMessageRegexes =
    [
        SqlServerMessageRegex_515_English(), // 515 British
        SqlServerMessageRegex_515_Czech(), // 515 čeština
        SqlServerMessageRegex_515_Danish(), // 515 Dansk
        SqlServerMessageRegex_515_German(), // 515 Deutsch
        SqlServerMessageRegex_515_Spanish(), // 515 Español
        SqlServerMessageRegex_515_French(), // 515 Français
        SqlServerMessageRegex_515_Italian(), // 515 Italiano
        SqlServerMessageRegex_515_Hungarian(), // 515 magyar
        SqlServerMessageRegex_515_Dutch(), // 515 Nederlands
        SqlServerMessageRegex_515_Norwegian(), // 515 norsk (bokmål)
        SqlServerMessageRegex_515_Polish(), // 515 polski
        SqlServerMessageRegex_515_Portuguese(), // 515 Português
        SqlServerMessageRegex_515_PortugueseBrazil(), // 515 Português (Brasil)
        SqlServerMessageRegex_515_Finnish(), // 515 Suomi
        SqlServerMessageRegex_515_Swedish(), // 515 Svenska
        SqlServerMessageRegex_515_Turkish(), // 515 Türkçe
        SqlServerMessageRegex_515_UsEnglish(), // 515 us_english
        SqlServerMessageRegex_515_Greek(), // 515 ελληνικά
        SqlServerMessageRegex_515_Russian(), // 515 русский
        SqlServerMessageRegex_515_Korean(), // 515 한국어
        SqlServerMessageRegex_515_Japanese(), // 515 日本語
        SqlServerMessageRegex_515_ChineseSimplified(), // 515 简体中文
        SqlServerMessageRegex_515_ChineseTraditional(), // 515 繁體中文
        SqlServerMessageRegex_547_English(), // 547 British
        SqlServerMessageRegex_547_Czech(), // 547 čeština
        SqlServerMessageRegex_547_Danish(), // 547 Dansk
        SqlServerMessageRegex_547_German(), // 547 Deutsch
        SqlServerMessageRegex_547_Spanish(), // 547 Español
        SqlServerMessageRegex_547_French(), // 547 Français
        SqlServerMessageRegex_547_Italian(), // 547 Italiano
        SqlServerMessageRegex_547_Hungarian(), // 547 magyar
        SqlServerMessageRegex_547_Dutch(), // 547 Nederlands
        SqlServerMessageRegex_547_Norwegian(), // 547 norsk (bokmål)
        SqlServerMessageRegex_547_Polish(), // 547 polski
        SqlServerMessageRegex_547_Portuguese(), // 547 Português
        SqlServerMessageRegex_547_PortugueseBrazil(), // 547 Português (Brasil)
        SqlServerMessageRegex_547_Finnish(), // 547 Suomi
        SqlServerMessageRegex_547_Swedish(), // 547 Svenska
        SqlServerMessageRegex_547_Turkish(), // 547 Türkçe
        SqlServerMessageRegex_547_UsEnglish(), // 547 us_english
        SqlServerMessageRegex_547_Greek(), // 547 ελληνικά
        SqlServerMessageRegex_547_Russian(), // 547 русский
        SqlServerMessageRegex_547_Korean(), // 547 한국어
        SqlServerMessageRegex_547_Japanese(), // 547 日本語
        SqlServerMessageRegex_547_ChineseSimplified(), // 547 简体中文
        SqlServerMessageRegex_547_ChineseTraditional(), // 547 繁體中文
        SqlServerMessageRegex_2601_English(), // 2601 British
        SqlServerMessageRegex_2601_Czech(), // 2601 čeština
        SqlServerMessageRegex_2601_Danish(), // 2601 Dansk
        SqlServerMessageRegex_2601_German(), // 2601 Deutsch
        SqlServerMessageRegex_2601_Spanish(), // 2601 Español
        SqlServerMessageRegex_2601_French(), // 2601 Français
        SqlServerMessageRegex_2601_Italian(), // 2601 Italiano
        SqlServerMessageRegex_2601_Hungarian(), // 2601 magyar
        SqlServerMessageRegex_2601_Dutch(), // 2601 Nederlands
        SqlServerMessageRegex_2601_Norwegian(), // 2601 norsk (bokmål)
        SqlServerMessageRegex_2601_Polish(), // 2601 polski
        SqlServerMessageRegex_2601_Portuguese(), // 2601 Português
        SqlServerMessageRegex_2601_PortugueseBrazil(), // 2601 Português (Brasil)
        SqlServerMessageRegex_2601_Finnish(), // 2601 Suomi
        SqlServerMessageRegex_2601_Swedish(), // 2601 Svenska
        SqlServerMessageRegex_2601_Turkish(), // 2601 Türkçe
        SqlServerMessageRegex_2601_UsEnglish(), // 2601 us_english
        SqlServerMessageRegex_2601_Greek(), // 2601 ελληνικά
        SqlServerMessageRegex_2601_Russian(), // 2601 русский
        SqlServerMessageRegex_2601_Korean(), // 2601 한국어
        SqlServerMessageRegex_2601_Japanese(), // 2601 日本語
        SqlServerMessageRegex_2601_ChineseSimplified(), // 2601 简体中文
        SqlServerMessageRegex_2601_ChineseTraditional(), // 2601 繁體中文
        SqlServerMessageRegex_2627_English(), // 2627 British
        SqlServerMessageRegex_2627_Czech(), // 2627 čeština
        SqlServerMessageRegex_2627_Danish(), // 2627 Dansk
        SqlServerMessageRegex_2627_German(), // 2627 Deutsch
        SqlServerMessageRegex_2627_Spanish(), // 2627 Español
        SqlServerMessageRegex_2627_French(), // 2627 Français
        SqlServerMessageRegex_2627_Italian(), // 2627 Italiano
        SqlServerMessageRegex_2627_Hungarian(), // 2627 magyar
        SqlServerMessageRegex_2627_Dutch(), // 2627 Nederlands
        SqlServerMessageRegex_2627_Norwegian(), // 2627 norsk (bokmål)
        SqlServerMessageRegex_2627_Polish(), // 2627 polski
        SqlServerMessageRegex_2627_Portuguese(), // 2627 Português
        SqlServerMessageRegex_2627_PortugueseBrazil(), // 2627 Português (Brasil)
        SqlServerMessageRegex_2627_Finnish(), // 2627 Suomi
        SqlServerMessageRegex_2627_Swedish(), // 2627 Svenska
        SqlServerMessageRegex_2627_Turkish(), // 2627 Türkçe
        SqlServerMessageRegex_2627_UsEnglish(), // 2627 us_english
        SqlServerMessageRegex_2627_Greek(), // 2627 ελληνικά
        SqlServerMessageRegex_2627_Russian(), // 2627 русский
        SqlServerMessageRegex_2627_Korean(), // 2627 한국어
        SqlServerMessageRegex_2627_Japanese(), // 2627 日本語
        SqlServerMessageRegex_2627_ChineseSimplified(), // 2627 简体中文
        SqlServerMessageRegex_2627_ChineseTraditional(), // 2627 繁體中文
    ];

    // 515 British
    [GeneratedRegex("^Cannot insert the value NULL into column '(?<column>.*?)', table '(?<table>.*?)'; column does not allow nulls\\. .*? fails\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_English();

    // 515 čeština
    [GeneratedRegex("^Do sloupce (?<column>.*?) tabulky (?<table>.*?) nelze vložit hodnotu NULL\\. Sloupec nepovoluje hodnoty NULL\\. Akce .*? se nezdařila\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Czech();

    // 515 Dansk
    [GeneratedRegex("^Værdien NULL kan ikke indsættes i kolonnen '(?<column>.*?)', tabellen '(?<table>.*?)'\\. Kolonnen tillader ikke null-værdier\\. .*? kan ikke udføres\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Danish();

    // 515 Deutsch
    [GeneratedRegex("^Der Wert NULL kann in die (?<column>.*?)-Spalte, (?<table>.*?)-Tabelle nicht eingefügt werden\\. Die Spalte lässt NULL-Werte nicht zu\\. Fehler bei .*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_German();

    // 515 Español
    [GeneratedRegex("^No se puede insertar el valor NULL en la columna '(?<column>.*?)', tabla '(?<table>.*?)'\\. La columna no admite valores NULL\\. Error de .*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Spanish();

    // 515 Français
    [GeneratedRegex("^Impossible d'insérer la valeur NULL dans la colonne '(?<column>.*?)', table '(?<table>.*?)'\\. Cette colonne n'accepte pas les valeurs NULL\\. Échec de .*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_French();

    // 515 Italiano
    [GeneratedRegex("^Non è possibile inserire il valore NULL nella colonna '(?<column>.*?)' della tabella '(?<table>.*?)'\\. La colonna non ammette valori Null\\. .*? avrà esito negativo\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Italian();

    // 515 magyar
    [GeneratedRegex("^A NULL érték nem szúrható be a\\(z\\) „(?<column>.*?)” oszlopba \\(„(?<table>.*?)” tábla\\), az oszlop nem engedni meg a null értékek használatát\\. A\\(z\\) .*? leáll\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Hungarian();

    // 515 Nederlands
    [GeneratedRegex("^Kan de waarde NULL niet invoegen in kolom (?<column>.*?), tabel (?<table>.*?)\\. De kolom staat geen NULL-waarden toe\\. .*? is mislukt\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Dutch();

    // 515 norsk (bokmål)
    [GeneratedRegex("^Kan ikke sett inn verdien NULL i kolonnen (?<column>.*?), tabellen (?<table>.*?)\\. Kolonnen tillater ikke null\\. .*? mislykkes\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Norwegian();

    // 515 polski
    [GeneratedRegex("^Nie można wstawić wartości NULL do kolumny „(?<column>.*?)” tabeli „(?<table>.*?)”, ponieważ kolumna nie pozwala na wprowadzanie takich wartości\\. .*? kończy się niepowodzeniem\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Polish();

    // 515 Português
    [GeneratedRegex("^Não é possível inserir o valor NULL na coluna '(?<column>.*?)', tabela '(?<table>.*?)'; a coluna não permite valores null\\. Falha de .*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Portuguese();

    // 515 Português (Brasil)
    [GeneratedRegex("^Não é possível inserir o valor NULL na coluna '(?<column>.*?)', tabela '(?<table>.*?)'; a coluna não permite nulos\\. Falha em .*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_PortugueseBrazil();

    // 515 Suomi
    [GeneratedRegex("^Arvoa NULL ei voi lisätä taulukon (?<table>.*?) sarakkeeseen (?<column>.*?), sillä sarake ei salli tyhjäarvoja\\. .*? virhettä\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Finnish();

    // 515 Svenska
    [GeneratedRegex("^Det går inte att infoga värdet NULL i kolumnen (?<column>.*?), tabellen (?<table>.*?); kolumnen medger inte null-värden\\. .*? misslyckas\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Swedish();

    // 515 Türkçe
    [GeneratedRegex("^NULL değeri '(?<table>.*?)' tablosunun '(?<column>.*?)' sütununa eklemez; sütun null değerlere izin vermiyor\\. .*? başarısız\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Turkish();

    // 515 us_english
    [GeneratedRegex("^Cannot insert the value NULL into column '(?<column>.*?)', table '(?<table>.*?)'; column does not allow nulls\\. .*? fails\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_UsEnglish();

    // 515 ελληνικά
    [GeneratedRegex("^Δεν είναι δυνατή η εισαγωγή της τιμής NULL στη στήλη '(?<column>.*?)', στον πίνακα '(?<table>.*?)', η στήλη δεν επιτρέπει τιμές null\\. Αποτυχία του .*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Greek();

    // 515 русский
    [GeneratedRegex("^Не удалось вставить значение NULL в столбец \"(?<column>.*?)\", таблицы \"(?<table>.*?)\"; в столбце запрещены значения NULL\\. Ошибка в .*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Russian();

    // 515 한국어
    [GeneratedRegex("^테이블 '(?<table>.*?)', 열 '(?<column>.*?)'에 NULL 값을 삽입할 수 없습니다\\. 열에는 NULL을 사용할 수 없습니다\\. .*?이\\(가\\) 실패했습니다\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Korean();

    // 515 日本語
    [GeneratedRegex("^テーブル '(?<table>.*?)' の列 '(?<column>.*?)' に値 NULL を挿入できません。この列では NULL 値が許可されていません。.*? は失敗します。$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_Japanese();

    // 515 简体中文
    [GeneratedRegex("^不能将值 NULL 插入列 '(?<column>.*?)'，表 '(?<table>.*?)'；列不允许有 Null 值。.*? 失败。$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_ChineseSimplified();

    // 515 繁體中文
    [GeneratedRegex("^無法插入 NULL 值到資料行 '(?<column>.*?)'，資料表 '(?<table>.*?)'; 資料行不得有 Null。.*? 失敗。$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_515_ChineseTraditional();

    // 547 British
    [GeneratedRegex("^The .*? statement conflicted with the .*? constraint \"(?<constraint>.*?)\"\\. The conflict occurred in database \".*?\", table \"(?<table>.*?)\".*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_English();

    // 547 čeština
    [GeneratedRegex("^Příkaz .*? způsobil konflikt s omezením .*? s názvem (?<constraint>.*?)\\. Ke konfliktu došlo v databázi .*?, tabulce (?<table>.*?), .*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Czech();

    // 547 Dansk
    [GeneratedRegex("^Sætningen .*? kom i konflikt med .*?-begrænsningen \"(?<constraint>.*?)\"\\. Konflikten opstod i databasen \".*?\", tabellen \"(?<table>.*?)\".*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Danish();

    // 547 Deutsch
    [GeneratedRegex("^Die .*?-Anweisung steht in Konflikt mit der .*?-Einschränkung \"(?<constraint>.*?)\"\\. Der Konflikt trat in der .*?-Datenbank, Tabelle \"(?<table>.*?)\".*?.*?.*? auf\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_German();

    // 547 Español
    [GeneratedRegex("^Instrucción .*? en conflicto con la restricción .*? '(?<constraint>.*?)'\\. El conflicto ha aparecido en la base de datos '.*?', tabla '(?<table>.*?)'.*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Spanish();

    // 547 Français
    [GeneratedRegex("^L'instruction .*? est en conflit avec la contrainte .*? \"(?<constraint>.*?)\"\\. Le conflit s'est produit dans la base de données \".*?\", table \"(?<table>.*?)\".*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_French();

    // 547 Italiano
    [GeneratedRegex("^L'istruzione .*? è in conflitto con il vincolo .*? \"(?<constraint>.*?)\"\\. Il conflitto si è verificato nella tabella \"(?<table>.*?)\".*?.*?.*? del database \".*?\"\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Italian();

    // 547 magyar
    [GeneratedRegex("^A\\(z\\) .*? utasítás ütközést eredményezett a\\(z\\) „(?<constraint>.*?)” .*? megkötéssel\\. Az ütközés előfordulásának helye: „.*?” adatbázis, „(?<table>.*?)”.*?.*?.*? tábla\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Hungarian();

    // 547 Nederlands
    [GeneratedRegex("^De instructie .*? is strijdig met de .*?-beperking (?<constraint>.*?)\\. Het conflict is opgetreden in database .*?, tabel (?<table>.*?).*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Dutch();

    // 547 norsk (bokmål)
    [GeneratedRegex("^Setningen .*? er i konflikt med begrensningen (?<constraint>.*?) i .*?\\. Konflikten oppstod i databasen .*?, tabell (?<table>.*?) .*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Norwegian();

    // 547 polski
    [GeneratedRegex("^Instrukcja .*? powoduje konflikt z ograniczeniem .*? „(?<constraint>.*?)”\\. Konflikt występuje w bazie danych „.*?” w tabeli „(?<table>.*?)”.*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Polish();

    // 547 Português
    [GeneratedRegex("^Conflito entre a instrução .*? e a restrição .*? \"(?<constraint>.*?)\"\\. O conflito ocorreu na base de dados \".*?\", tabela \"(?<table>.*?)\".*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Portuguese();

    // 547 Português (Brasil)
    [GeneratedRegex("^A instrução .*? conflitou com a restrição do .*? \"(?<constraint>.*?)\"\\. O conflito ocorreu no banco de dados \".*?\", tabela \"(?<table>.*?)\".*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_PortugueseBrazil();

    // 547 Suomi
    [GeneratedRegex("^Lauseke .*? oli ristiriidassa kohteen .*? rajoituksen (?<constraint>.*?) kanssa\\. Ristiriita ilmeni tietokannassa .*? ja taulukossa (?<table>.*?).*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Finnish();

    // 547 Svenska
    [GeneratedRegex("^En konflikt inträffade mellan instruktionen .*? och .*?-begränsningen \"(?<constraint>.*?)\"\\. Konflikten inträffade i databasen \".*?\", tabell \"(?<table>.*?)\".*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Swedish();

    // 547 Türkçe
    [GeneratedRegex("^.*? deyimi .*? kısıtlaması \"(?<constraint>.*?)\" ile çakıştı\\. \".*?\" veritabanı, \"(?<table>.*?)\".*?.*?.*? tablosu içinde çakışma oluştu\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Turkish();

    // 547 us_english
    [GeneratedRegex("^The .*? statement conflicted with the .*? constraint \"(?<constraint>.*?)\"\\. The conflict occurred in database \".*?\", table \"(?<table>.*?)\".*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_UsEnglish();

    // 547 ελληνικά
    [GeneratedRegex("^Η πρόταση .*? δημιούργησε διένεξη με τον περιορισμό .*? \"(?<constraint>.*?)\"\\. Η διένεξη παρουσιάστηκε στη βάση δεδομένων \".*?\", στον πίνακα \"(?<table>.*?)\".*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Greek();

    // 547 русский
    [GeneratedRegex("^Конфликт инструкции .*? с ограничением .*? \"(?<constraint>.*?)\"\\. Конфликт произошел в базе данных \".*?\", таблица \"(?<table>.*?)\".*?.*?.*?\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Russian();

    // 547 한국어
    [GeneratedRegex("^.*? 문이 .*? 제약 조건 \"(?<constraint>.*?)\"과\\(와\\) 충돌했습니다\\. 데이터베이스 \".*?\", 테이블 \"(?<table>.*?)\".*?.*?.*?에서 충돌이 발생했습니다\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Korean();

    // 547 日本語
    [GeneratedRegex("^.*? ステートメントは .*? 制約 \"(?<constraint>.*?)\" と競合しています。競合が発生したのは、データベース \".*?\"、テーブル \"(?<table>.*?)\".*?.*?.*? です。$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_Japanese();

    // 547 简体中文
    [GeneratedRegex("^.*? 语句与 .*? 约束\"(?<constraint>.*?)\"冲突。该冲突发生于数据库\".*?\"，表\"(?<table>.*?)\".*?.*?.*?。$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_ChineseSimplified();

    // 547 繁體中文
    [GeneratedRegex("^.*? 陳述式與 .*? 條件約束 \"(?<constraint>.*?)\" 衝突。衝突發生在資料庫 \".*?\"，資料表 \"(?<table>.*?)\".*?.*?.*?。$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_547_ChineseTraditional();

    // 2601 British
    [GeneratedRegex("^Cannot insert duplicate key row in object '(?<table>.*?)' with unique index '(?<constraint>.*?)'\\. The duplicate key value is (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_English();

    // 2601 čeština
    [GeneratedRegex("^Do objektu (?<table>.*?) s jedinečným indexem (?<constraint>.*?) nelze vložit duplicitní klíčový řádek\\.Hodnota duplicitního klíče je(?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Czech();

    // 2601 Dansk
    [GeneratedRegex("^Dubletnøglerække kan ikke indsættes i objektet '(?<table>.*?)' med det entydige indeks '(?<constraint>.*?)'\\. Dubletnøgleværdien er (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Danish();

    // 2601 Deutsch
    [GeneratedRegex("^Eine Zeile mit doppeltem Schlüssel kann in das (?<table>.*?)-Objekt mit dem eindeutigen (?<constraint>.*?)-Index nicht eingefügt werden\\. Der doppelte Schlüsselwert ist (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_German();

    // 2601 Español
    [GeneratedRegex("^No se puede insertar una fila de clave duplicada en el objeto '(?<table>.*?)' con índice único '(?<constraint>.*?)'\\. El valor de la clave duplicada es (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Spanish();

    // 2601 Français
    [GeneratedRegex("^Impossible d'insérer une ligne de clé en double dans l'objet « (?<table>.*?) » avec un index unique « (?<constraint>.*?) »\\. Valeur de clé dupliquée : (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_French();

    // 2601 Italiano
    [GeneratedRegex("^Non è possibile inserire la riga di chiave duplicata nell'oggetto '(?<table>.*?)' con indice univoco '(?<constraint>.*?)'\\. Valore della chiave duplicata: (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Italian();

    // 2601 magyar
    [GeneratedRegex("^Nem szúrható be duplikált kulcssor az egyedi „(?<constraint>.*?)” indexszel rendelkező „(?<table>.*?)” objektumba\\. A duplikált kulcsérték (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Hungarian();

    // 2601 Nederlands
    [GeneratedRegex("^Kan geen rij met dubbele sleutel invoegen in object (?<table>.*?) met unieke index (?<constraint>.*?)\\. De dubbele sleutelwaarde is (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Dutch();

    // 2601 norsk (bokmål)
    [GeneratedRegex("^Kan ikke sette inn duplisert nøkkelrad i objektet (?<table>.*?) med den unike indeksen (?<constraint>.*?)\\. Verdien til den dupliserte nøkkelen er (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Norwegian();

    // 2601 polski
    [GeneratedRegex("^Nie można wstawić wiersza zduplikowanego klucza w obiekcie (?<table>.*?) o unikatowym indeksie (?<constraint>.*?)\\. Wartość zduplikowanego klucza to (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Polish();

    // 2601 Português
    [GeneratedRegex("^Não é possível inserir uma linha de chave duplicada no objeto '(?<table>.*?)' com o índice exclusivo '(?<constraint>.*?)'\\. O valor de chave duplicada é (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Portuguese();

    // 2601 Português (Brasil)
    [GeneratedRegex("^Não é possível inserir uma linha de chave duplicada no objeto '(?<table>.*?)' com índice exclusivo '(?<constraint>.*?)'\\. O valor de chave duplicada é (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_PortugueseBrazil();

    // 2601 Suomi
    [GeneratedRegex("^Avainrivin kaksoiskappaletta ei voi lisätä objektiin (?<table>.*?) käyttäen yksilöivää indeksiä (?<constraint>.*?)\\. Avaimen arvon kaksoiskappale on (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Finnish();

    // 2601 Svenska
    [GeneratedRegex("^Det går inte att infoga rad med dubblettnyckel i objektet (?<table>.*?) med det unika indexet (?<constraint>.*?)\\. Dubblettnyckelvärdet är (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Swedish();

    // 2601 Türkçe
    [GeneratedRegex("^Benzersiz '(?<constraint>.*?)' dizinine sahip '(?<table>.*?)' nesnesine yinelenen anahtar satırı eklenemiyor\\. Yinelenen anahtar değeri: (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Turkish();

    // 2601 us_english
    [GeneratedRegex("^Cannot insert duplicate key row in object '(?<table>.*?)' with unique index '(?<constraint>.*?)'\\. The duplicate key value is (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_UsEnglish();

    // 2601 ελληνικά
    [GeneratedRegex("^Δεν είναι δυνατή η εισαγωγή της σειράς διπλότυπου κλειδιού στο αντικείμενο '(?<table>.*?)' με μοναδικό ευρετήριο '(?<constraint>.*?)'\\. Η τιμή του διπλότυπου κλειδιού είναι (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Greek();

    // 2601 русский
    [GeneratedRegex("^Не удается вставить повторяющуюся строку ключа в объект \"(?<table>.*?)\" с уникальным индексом \"(?<constraint>.*?)\"\\. Повторяющееся значение ключа: (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Russian();

    // 2601 한국어
    [GeneratedRegex("^고유 인덱스 '(?<constraint>.*?)'을\\(를\\) 포함하는 개체 '(?<table>.*?)'에 중복 키 행을 삽입할 수 없습니다\\. 중복 키 값은 (?<value>.*?)입니다\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Korean();

    // 2601 日本語
    [GeneratedRegex("^一意インデックス '(?<constraint>.*?)' を含むオブジェクト '(?<table>.*?)' には重複するキー行を挿入できません。重複するキーの値は (?<value>.*?) です。$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_Japanese();

    // 2601 简体中文
    [GeneratedRegex("^不能在具有唯一索引“(?<constraint>.*?)”的对象“(?<table>.*?)”中插入重复键的行。重复键值为 (?<value>.*?)。$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_ChineseSimplified();

    // 2601 繁體中文
    [GeneratedRegex("^無法以唯一索引 '(?<constraint>.*?)' 在物件 '(?<table>.*?)' 中插入重複的索引鍵資料列。重複的索引鍵值是 (?<value>.*?)。$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2601_ChineseTraditional();

    // 2627 British
    [GeneratedRegex("^Violation of .*? constraint '(?<constraint>.*?)'\\. Cannot insert duplicate key in object '(?<table>.*?)'\\. The duplicate key value is (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_English();

    // 2627 čeština
    [GeneratedRegex("^Porušení omezení .*? (?<constraint>.*?)\\. Nelze vložit duplicitní klíč do objektu (?<table>.*?)\\.Hodnota duplicitního klíče je(?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Czech();

    // 2627 Dansk
    [GeneratedRegex("^Overtrædelse af .*?-begrænsningen '(?<constraint>.*?)'\\. Dubletnøgle kan ikke indsættes i objektet '(?<table>.*?)'\\. Dubletnøgleværdien er (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Danish();

    // 2627 Deutsch
    [GeneratedRegex("^Verletzung der .*?-Einschränkung \"(?<constraint>.*?)\"\\. Ein doppelter Schlüssel kann in das (?<table>.*?)-Objekt nicht eingefügt werden\\. Der doppelte Schlüsselwert ist (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_German();

    // 2627 Español
    [GeneratedRegex("^Infracción de la restricción .*? '(?<constraint>.*?)'\\. No se puede insertar una clave duplicada en el objeto '(?<table>.*?)'\\. El valor de la clave duplicada es (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Spanish();

    // 2627 Français
    [GeneratedRegex("^Violation de la contrainte .*? « (?<constraint>.*?) »\\. Impossible d'insérer une clé en double dans l'objet « (?<table>.*?) »\\. Valeur de clé dupliquée : (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_French();

    // 2627 Italiano
    [GeneratedRegex("^Violazione del vincolo .*? '(?<constraint>.*?)'\\. Non è possibile inserire la chiave duplicata nell'oggetto '(?<table>.*?)'\\. Valore della chiave duplicata: (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Italian();

    // 2627 magyar
    [GeneratedRegex("^A következő .*? korlátozás megsértése: „(?<constraint>.*?)”\\. Nem szúrható be duplikált kulcs a következő objektumba: „(?<table>.*?)”\\. A duplikált kulcsérték (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Hungarian();

    // 2627 Nederlands
    [GeneratedRegex("^Er is een schending van de .*?-beperking (?<constraint>.*?) opgetreden\\. Kan geen dubbele sleutel invoegen in het object (?<table>.*?)\\. De dubbele sleutelwaarde is (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Dutch();

    // 2627 norsk (bokmål)
    [GeneratedRegex("^Brudd på .*?-begrensningen (?<constraint>.*?)\\. Kan ikke sette inn duplisert nøkkel i objektet (?<table>.*?)\\. Verdien til den dupliserte nøkkelen er (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Norwegian();

    // 2627 polski
    [GeneratedRegex("^Naruszenie podreguły .*? ograniczenia „(?<constraint>.*?)”\\. Nie można wstawić zduplikowanego klucza w obiekcie „(?<table>.*?)”\\. Wartość zduplikowanego klucza to (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Polish();

    // 2627 Português
    [GeneratedRegex("^Violação da restrição .*? '(?<constraint>.*?)'\\. Não é possível inserir uma chave duplicada no objeto '(?<table>.*?)'\\. O valor de chave duplicada é (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Portuguese();

    // 2627 Português (Brasil)
    [GeneratedRegex("^Violação da restrição .*? '(?<constraint>.*?)'\\. Não é possível inserir a chave duplicada no objeto '(?<table>.*?)'\\. O valor de chave duplicada é (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_PortugueseBrazil();

    // 2627 Suomi
    [GeneratedRegex("^Kohteen .*? rajoituksen (?<constraint>.*?) rikkomus\\. Avaimen kaksoiskappaletta ei voi lisätä objektiin (?<table>.*?)\\. Avaimen arvon kaksoiskappale on (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Finnish();

    // 2627 Svenska
    [GeneratedRegex("^Överträdelse av .*? begränsning (?<constraint>.*?)\\. Det går inte att infoga dubblettnyckel i objektet (?<table>.*?)\\. Dubblettnyckelvärdet är (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Swedish();

    // 2627 Türkçe
    [GeneratedRegex("^.*? kısıtlaması '(?<constraint>.*?)' ihlal edildi\\. '(?<table>.*?)' nesnesine yinelenen anahtar eklenemiyor\\. Yinelenen anahtar değeri: (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Turkish();

    // 2627 us_english
    [GeneratedRegex("^Violation of .*? constraint '(?<constraint>.*?)'\\. Cannot insert duplicate key in object '(?<table>.*?)'\\. The duplicate key value is (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_UsEnglish();

    // 2627 ελληνικά
    [GeneratedRegex("^Παραβίαση του περιορισμού .*? '(?<constraint>.*?)'\\. Δεν είναι δυνατή η εισαγωγή διπλότυπου κλειδιού στο αντικείμενο '(?<table>.*?)'\\. Η τιμή του διπλότυπου κλειδιού είναι (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Greek();

    // 2627 русский
    [GeneratedRegex("^Нарушено \"(?<constraint>.*?)\" ограничения .*?\\. Не удается вставить повторяющийся ключ в объект \"(?<table>.*?)\"\\. Повторяющееся значение ключа: (?<value>.*?)\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Russian();

    // 2627 한국어
    [GeneratedRegex("^.*? 제약 조건 '(?<constraint>.*?)'을\\(를\\) 위반했습니다\\. 개체 '(?<table>.*?)'에 중복 키를 삽입할 수 없습니다\\. 중복 키 값은 (?<value>.*?)입니다\\.$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Korean();

    // 2627 日本語
    [GeneratedRegex("^制約 '(?<constraint>.*?)' の .*? 違反。オブジェクト '(?<table>.*?)' には重複するキーを挿入できません。重複するキーの値は (?<value>.*?) です。$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_Japanese();

    // 2627 简体中文
    [GeneratedRegex("^违反了 .*? 约束“(?<constraint>.*?)”。不能在对象“(?<table>.*?)”中插入重复键。重复键值为 (?<value>.*?)。$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_ChineseSimplified();

    // 2627 繁體中文
    [GeneratedRegex("^違反 .*? 條件約束 '(?<constraint>.*?)'。無法在物件 '(?<table>.*?)' 中插入重複的索引鍵。重複的索引鍵值是 (?<value>.*?)。$", RegexOptions.CultureInvariant)]
    private static partial Regex SqlServerMessageRegex_2627_ChineseTraditional();

}
