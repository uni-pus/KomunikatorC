1. Pobieranie adresu IP i Portu servera z pliku config.xml (plik XML) 
	jeżeli plik nie istnieje to pobranie ustawień z sieci: z adresu: http://dlugiego.net/pus/servers.php
	(chodzi o to, by do testów móc używać pliku config.xml, a w wersji "produkcyjnej" łączył się ze stroną i pobierał bezpośrednio z sieci)
2. ogólny wygląd aplikacji
3. Listę kontaktów, np trzymanie w ListBoxie, czyli dodanie nowego rozmówcy do ListBoxa (lub rozmówca sam siebie dodaje poprzez napisanie do tego klienta)
	oczywiście możliwość zamknięcia danego kontaktu, po kliknięciu na dany kontakt trzeba będzie zapisać treść aktualnej rozmowy do "pamięci" i do okna rozmowy
	skopiować treść rozmowy z nowym rozmówcą (oczywiście przy pierwszej wiadomości "treść" rozmowy pusta.
4. okno z wiadomościami przerobić z aktualnego (TextBlock) na WebBrowser - można fajnie formatować wyświetlane rozmowy 
5. 