# Betfair SP Scraper

The purpose of this application is to automate downloading of the Betfair SP csv files available at:

* [Directory Listing of Betfair price files](https://promo.betfair.com/betfairsp/prices/)

Downloads are done asynchronously so it can download multiple files at once.

Coded in C#

## Running the program

The program uses a command-line interface. After starting the the program the following commands are available to the user:

```
start
exit
stoptasks
```

#### If the user types 'start' the subsequent data must be provided:

* Country: 'uk' (United Kingdom), 'ire' (Ireland), 'aus' (Australia), 'usa' (USA), 'rsa' (South Africa)
* Bet Type: 'win', 'place'
* Greyhound: 'yes', 'no'
* Start and End Date: dd/mm/yyyy

An example of the inputs would be:
```
Country (uk, ire, aus, usa, rsa): aus
Bet type (win, place): win
Greyhound Only? (yes, no): yes
Start Date (dd/mm/yyyy):01/03/2018
End Date (dd/mm/yyyy):31/07/2018
```
This would download all the Australian Greyhound racing csv files from the 1st of March 2018 to the 31st of July 2018 (win data only)

#### If the user types 'exit' the program will terminate all download threads and exit the program.

#### If the user types 'stoptasks' the program will terminate all download threads.

## Built With

* [Visual Studio 2013](https://visualstudio.microsoft.com/vs/)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
