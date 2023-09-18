# Playwright Web Page Merge

Merge web pages together into a single pdf document. This code sample make use of your interactive Microsoft Edge browser to saves pages into a single pdf file with an optional cover page.

## Notes

Some notes and assumptions

1. As Microsoft Edge browser is used pages behind a security auth prompt can be navigated to and saved.

2. The app assumes tha the pages to be saved use a base URL set in App.config

3. Uses interactive mode for any login requirements

4. Assume Microsoft Edge is installed in default user profile on Windows

5. Visits the home page first to allow for login. Following pages will used login browser credentials

7. .Net 7.0 SDK of greater is installed 

## Getting Started

1. Ensure that you have the [Microsoft .Net SDK Installed](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks)

2. Ensure that the installed SDK is in your path

3. Edit the App.config with the the site and pages to combine

|Setting|Description|
|-----=-|-----------|
|BaseUrl|The web page with {0} wildcard to visit
|Home|The first page to include in the merged document after the cover
|Pages|A comma delimited list of pages to include
|Cover|A pdf document to include as cover page

4. Build the code

```cmd
cd src
dotnet build    
```

5. Ensure Playwright is installed

```cmd
# Install required browsers - replace netX with actual output folder name, e.g. net6.0.
pwsh bin/Debug/netX/playwright.ps1 install 
``````

4. Run the merge

```cmd
dotnet run
```
