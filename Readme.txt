 --- Istruzioni preliminari per l'installazione della Dashboard EmulEP di Andrea Barbadoro --- 

La Dashbord EmulEP è il software di controllo dell'emulatore Hardware progettato e realizzato da Paolo Carrer.
La scheda dell'emulatore si connette al PC tramite cavo USB Mini-B.
Normalmente la scheda viene alimentata direttamente dallo zoccolo della EPROM che sta emulando, in questo caso non deve essere montato D9 e SJ1 deve risultare aperto in modo da isolare elettricamente il PC dal sistema che si sta debuggando.

Quando si collega la scheda al PC per la prima volta Windows cerca i driver per USB del modulo FTDI montato a bordo dell'emulatore.
Se windows non dovesse riuscire ad installare autonomamente i driver occorre scaricarli e installarli manualmente dal sito del produttore:
https://ftdichip.com/drivers/

Una volta installati i driver in windows, si può procedere ad installare il software della dashboard lanciando l'eseguibile:

EmulEPInstall.exe 

che installerà la versione a 32 bit nel sistema Windows.

Successivamente per utilizzare il compilatore assembler Z80 Zasm, integrato nella dashboard, occorre scaricare la versione più recente dal sito:

https://k1.spdns.de/Develop/Projects/zasm/Distributions/

Windows 32 bit:
https://k1.spdns.de/Develop/Projects/zasm/Distributions/zasm-4.4.12-win32%20%5Bturbocat2001%5D.zip

Windows 64bit:
https://k1.spdns.de/Develop/Projects/zasm/Distributions/zasm-4.4.12-win64%20%5Bturbocat2001%5D.zip

e poi aggiungere il path in cui è stata scaricata la cartella zasm-4.4.12-windows nelle variabili d'ambiente seguendo questo percorso:

Pannello di controllo - Sistema - Impostazioni di sistema avanzate - Scheda avanzate - Variabili d'ambiente - Variabili dell'utente o Variabili di sistema - Selezionare Path - Modifica 

Aggiungere quindi una riga con il Path completo, esempio:

D\Andrea\Elettronica\Z80ne\Tools Software\Zasm\zasm-4.4.12-windows

enjoy

