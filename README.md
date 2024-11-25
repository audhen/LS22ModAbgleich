# Änderungen

Ich hab es nur Universeller gemacht, da LS 22 und der LS 25 die selbe Webseite teilen und die API gleich ist, braucht es nur noch den Mod Ordner zu ändern. Diesen habe ich in der Konfigurationsdatei angepasst.

# Originaler Text
### LS22ModAbgleich
Dedicated Server only.

Da man oft nicht nur Mods aus dem offiziellen Modhub von Giants verwendet, sondern auch gerne aus anderen Quellen/selbst Mods erstellt, ist es mit der Verteilung und Aktualisierung dieser oft nicht ganz einfach.
Entweder gibt es eine lose Linksammlung, ein paar Cloudordner, oder, oder, oder...

Am Ende heißt es wieder: Irgendein Mod fehlt mir? Wo bekomm ich den wieder her?
Oh, ich hab gestern alle Mods aktualisiert bei mir im Spiel - war das nicht gut?

Wenn man nun im Besitz des Dedicated Servers mit Webinterface ist, so kann man mit diesem Tool die Modverteilung vereinfachen.

Ihr müsst euch nur den Link zu "Link XML" aus dem Webinterface unter Einstellungen -> Verschiedenes schnappen.
Ferner müsst ihr den öffentlichen Mod Download unter "Öffentlicher Mod Download" aktivieren und dort den Link unter "Link zu aktiven Mods" kopieren und anpassen. Am Ende wird aus "mods.html" nur noch "mods".

Diese Einträge müssen dann entsprechend der Hinweise in die .exe.config eingetragen werden.

