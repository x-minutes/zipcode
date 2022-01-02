# ZipCode
## A tool for compressing VS projects

With great tools like GitHub for sharing source code, we sometimes forget that there are sometimes that we need to compress a code folder/solution to share with others. There are many ways to compress a Visual Studio solution/project to be able to share it.  As someone that teaches programming, I have seen many different ways that people have compressed their solutions.  Many people forget to get rid of the extra files and folders that are not needed.  This has lead to 3G zip files for downloading 25 students projects.  Zipping up this many projects and it being 2G+ per week for assignments per class is a little too much.  

ZipCode was developed to get rid of the extra files and folders so that the download can be as small and efficient as possible. This will work on any system where you have .NET6 installed.

Installing zipcode can be done using the nuget servers with the following command:

> dotnet tool install --global zipcode

If you are building it locally and want to install the local version then use the following command:

> dotnet tool install --global --add-source ./nupkg zipcode

To compress a project you can just run zipcode from a command prompt and add the project folder name and it will create a timestamped zip file for you.

> zipcode MyProject

If you do not want the timestamping then you can add the --nodate option

> zipcode --nodate MyProject

If you want to use zipcode to also unzip a project then you can add the --unzip option and provide the filename to unzip.

> zipcode --unzip myproject.zip

