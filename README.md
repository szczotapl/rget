# rGet - Command-Line File Downloader

## How to install:
1. Download [zip with exe and dll](bin/rget.zip)
2. Unpack these files to 'C:\Windows' directory or any other added to 'PATH'
3. Done! you can now use rget inside command prompt!

## Usage:
* rget <url> [options]


## Options:
* -o, --output <output_file>     Specify the output file path.
* -h, --headers <headers>        Specify custom headers (comma-separated).
* -c, --concurrent <downloads>   Number of concurrent downloads (default is 1).
* -t, --timeout <seconds>        Timeout in seconds for each request (default is 30).

## Examples:
1. Download and save to file:
* rget https://riviox.is-a.dev/helloworld.txt -o helloworld.txt
2. Download with custom headers:
* rget https://riviox.is-a.dev/helloworld.txt -h "Header1: Value1,Header2: Value2"

## Contributing:
* Contributions are welcome! Report issues or make suggestions through GitHub.

## License:
* This project is licensed under the MIT License - see the LICENSE file for details.
