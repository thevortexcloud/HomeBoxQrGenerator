# Introduction

This is a simple command line program to generate QR codes for [HomeBox](https://github.com/sysadminsmedia/homebox). As the built in QR code generator is fairly limited in terms of layout and does not support generating QR codes for anything but items

# How to compile

1. You will need a copy of the [dotnet 9.0 SDK](https://dotnet.microsoft.com/en-us/download) for your platform if you do not have it already.
2. Download a copy of the source code either by running `git clone https://github.com/thevortexcloud/HomeBoxQrGenerator.git` in a terminal, or downloading the [zip file](https://github.com/thevortexcloud/HomeBoxQrGenerator/archive/refs/heads/master.zip) from github and extract the files somewhere.
3. Once you have downloaded everything, open a terminal/cmd/powershell window in the location where you put the source code.
4. Run `dotnet build`
5. Assuming it completes without errors, you should be able to navigate to the `HomeBoxQrGenerator.Cli/bin/` folder to find the compiled output.
6. You have now compiled the program

# Running

All functionality can be executed by running the `HomeBoxQrGenerator.Cli.exe` file. This is a command line program as such it is expected to be run from a terminal. The following options are currently supported

| Option     | Name     | Description                                                                                                                                                                                                                                            |
|------------|----------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| --type     | Type     | Changes the type of label to be generated from HomeBox. Supported values are `item`,`location`. NOTE: Type values **ARE** case sensitive                                                                                                               |
| --host     | Host     | The server host of the HomeBox server. EG `https://demo.homebox.software/`                                                                                                                                                                             |
| --username | Username | The username to use to login to the HomeBox server                                                                                                                                                                                                     |
| --password | Password | The password to use to login to the HomeBox Server                                                                                                                                                                                                     |
| --query    | Query    | Used to specify what thing specifically should be returned from the server. EG if this is an item id it will result in a QR code being generated for the given item id. Some types also allow for searching for values instead of using a specific id. |
| --output   | Output   | The name of the file and location it should be outputted to. EG `./code.png`. NOTE: Only PNG files will be outputted, regardless of file extension                                                                                                     |

For example to generate a item label for the Smart Rocker Light Dimmer on the demo site you could do:

`HomeBoxQrGenerator.Cli.exe --type item --host https://demo.homebox.software/ --username demo@example.com --password demo --query dimmer --output ./code.png`
