# File Downloader

**FileDownloader** is a simple HTTP REST SPI service built with .NET Core 3.x, that allows clients to schedule and track multiple files download jobs.

### Design Overview

![alt text](doc/Design%20Overview.png "Design Overview")

**Note:**
> For simplicity proper **Persistent Queue Manager** is not included. Instead, In-memory Queue is exposed directly to REST API Controller via Dependency Injection. DO NOT USE THIS CODE IN PRODUCTION!!

**Note:**
> Current implementation does not retry to download files. Full implementation should use retry logic with **exponential backoff**.

### Key Files

| File          | Description |
| ------------- | ----------- |
| ./src/FileDownload.WinService/**Program.cs** | Application startup and host initialization logic. |
| ./src/FileDownload.Data/**HostBuilderExtensions.cs** | Registers in-memory relational database as primary data source for the application. Means all data is purged when application stopped. |
| ./src/FileDownload.Services/**HostBuilderExtensions.cs** | Registers **BlockingCollection<T>** as concurrent in-memory queue. |
| ./src/FileDownload.Services/FileDownload/**FileDownloadService.cs** | File download logic. Service uses **PLINQ** to process jobs and files in parallel. |
| ./src/FileDownload.Api/Controllers/**JobsController.cs** | REST API controller to create/read JOB(s). |

## Clone

```bash
> git clone https://github.com/ghen/filedown.git filedown
```

## Build

```bash
> cd filedown
> dotnet build
```

## Test

```bash
> dotnet test
```

## Run

For local testing (and production troubleshooting) application can be run in **console mode**. In this mode application will run from current working directory (affects relative paths resolution if used in configuration), and will terminate immediately upon current console session completion.

* To run application in **console mode** use ***--console*** command line switch:

  ```bash
  > cd ./src/FileDownload.WinService/bin/x64/Debug/netcoreapp3.1/win10-x64
  > filedown --console
  ```

* To exit application use ***Ctrl+C*** control sequence, or terminate current console session.

**Note:**
> During development application can be run from **Microsoft Visual Studio** IDE directly. It will execute in **console mode** by default.

## Service Installation

### Prerequisites

* **Windows 7 (64 bit)** or higher
* **.NET Core 3.1 Runtime** or later

Application can be installed as a local service using scripts provided in **./scripts**. No special tools are needed (other than **.NET Core 3.1 Runtime** being installed). User would also require elevated rights in order to install or uninstall service under the **Windows** operating system.

## Configuration

Application can be configured in all supported **.NET Core 3.1** manners. The easiest way would be to alter **./webhost.json** and **./services.json** files:

[services.json]

```json
{
  "FileDownload" : {
    "jobs": 3,
    "threads": 3,
    "downloads": "./downloads"
  }
}
```

[webhost.json]

```json
{
  "urls": "http://localhost:8080/api"
}
```

Supported settings:

| Path			     | Required | Type        | Description |
| -------------- |:--------:|:-----------:| ----------- |
| **urls**	     | Req      | String(1024) | HTTP REST API path to listen for incoming WEB requests for. |
| **jobs**	     | Req      | Number      | Number of maximum allowed parallel jobs. |
| **threads**    | Req      | Number      | Default number of threads per job. |
| **downloads**  | Req      | String(1024) | Path to the local folder to store downloaded files. |

## Logs

**Note:**
> Default log files location is **./logs**.

Structured logs are sent to console (in console mode only), as well as saved to local file system as instructed in **./nlog.config** file. Application should also be configured to have sufficient privileges to access logs files location.

![alt text](doc/Console%20Output.png "Console Output")

## REST API

HTTP REST API provided to all supporting clients.

### JSON vs XML

All data can be sent/received either in JSON or XML format.

Request format is defined by  **Content-Type** HTTP header, and response format is defined by **Accept** HTTP header correspondingly ([link](https://en.wikipedia.org/wiki/List_of_HTTP_header_fields)). For example:

    [Request]
    Content-Type: application/json
    Accept: application/json

If desired format is not provided, **application/json** is used by default.

### Common Data Structures

**File Link** details:

| Property			 | Required | Type        | Description |
| -------------- |:--------:|:-----------:| ----------- |
| **filename**	 | Req      | String(128) | Unique file name (unique in scope of current job only). |
| **link**	     | Req      | String(1024) | Link to external file to be downloaded. |

**Job Status** enumeration:

| Value          | Description |
| -------------- | ----------- |
| **Created**    | Job has been accepted (scheduled). |
| **Processing** | Job is currently running. |
| **Complete**   | Job has been completed. |

**Job** details:

| Property			 | Required | Type        | Description |
| -------------- |:--------:|:-----------:| ----------- |
| **id**	       | Req      | String(24)  | Unique job identifier (Base64-encoded UUID). Generated by the system. |
| **status**     | Req      | Enum        | **Job Status**. |
| **threads**	   | Req      | Number      | Level of parallelism. |
| **files**      | Req      | List        | List of **Files**. |

**File** details:

| Property			 | Required | Type        | Description |
| -------------- |:--------:|:-----------:| ----------- |
| **filename**	 | Req      | String(128) | Unique file name (unique in scope of current job only). |
| **link**	     | Req      | String(1024) | Link to external file to be downloaded. |
| **started**	   |          | Date        | Date and Time (ISO 8601) when file download started. Set to *<null>* until download started. |
| **finished**	 |          | Date        | Date and Time (ISO 8601) when file download finished. Set to *<null>* until download finished (or faileD). |
| **size**	     |          | Number      | File size (bytes) to download (where available). Set to *<null>* until download srtarted. |
| **bytes**	     |          | Number      | Total bytes downloaded (if successful). Set to *<null>* unless download finished successfully. |
| **error**	     |          | String(1024) | Error message (if download failed). Set to *<null>* unless download failed with error. |

### [GET] /jobs/{id}

Requests job status and file(s) download stats.

**Request**

| Parameter         | Required | Type        | Description |
| ----------------- |:--------:|:-----------:| ----------- |
| **id**            | Req      | String(24)  | \[URL] Unique job identifier. |

**Response**

| HTTP Status Code              | Description |
| ----------------------------- | ----------- |
| **200 OK**                    | **Job** details are sent back in response body. |
| **400 Bad Request**           | Incorrect (malformed) request. |
| **403 Not Found**             | Requested job was not found. |

**Example**

Request:

```
GET /api/jobs/Vgjzu0q9SBqLFNJv0m13dw HTTP/1.1
Accept: application/json
```

Response:

```
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
```

```json
{
   "id":"Vgjzu0q9SBqLFNJv0m13dw",
   "status":"Processing",
   "threads":5,
   "files":[
      {
         "filename":"Solar System.jpg",
         "link":"https://github.com/ghen/filedown/raw/master/test/resources/Solar System.jpg",
         "started":"2020-03-05T01:30:43.916+03:00",
         "finished":"2020-03-05T01:31:03.620+03:00",
         "size":83327,
         "bytes":83327
      },
      {
         "filename":"Bypass Capacitors.pdf",
         "link":"https://github.com/ghen/filedown/raw/master/test/resources/Bypass Capacitors.pdf",
         "started":"2020-03-05T01:30:43.916+03:00",
         "size":549158,
         "bytes":171128
      },
      {
         "filename":"Ops.tmp",
         "link":"https://github.com/ghen/filedown/raw/master/test/resources/Ops.tmp",
         "started":"2020-03-05T01:30:43.917+03:00",
         "finished":"2020-03-05T01:30:44.982+03:00",
         "size":0,
         "error":"Response status code does not indicate success: 404 (Not Found)."
      }
   ]
}
```

**Note:**
> As shown in the example above:
> * **Solar System.jpg** has been downloaded successfully
> * **Bypass Capacitors.pdf** download is still in progress
> * **Ops.tmp** download failed with error

### [POST] /jobs

Schedules new job to download one or more files.

**Request**

| Parameter         | Required | Type        | Description |
| ----------------- |:--------:|:-----------:| ----------- |
| **threads**	      | Req      | Number      | Level of parallelism. Should be a positive number. If set to **0** (zero) - default system settings will be used. Maximum allowed number of threads (per job) is **24**. |
| **links**	        | Req      | List        | List of **File Links**. |

**Response**

| HTTP Status Code              | Description |
| ----------------------------- | ----------- |
| **200 OK**                    | New **Job** details are sent back in response body. |
| **400 Bad Request**           | Incorrect (malformed) request. |

**Example**

Request:

```
POST /api/jobs HTTP/1.1
Accept: application/json
Content-Type: application/json
```

```json
{
  "threads": 5,
  "links": [
    { 
      "filename": "Solar System.jpg",
      "link": "https://github.com/ghen/filedown/raw/master/test/resources/Solar System.jpg"
    },
    { 
      "filename": "Bypass Capacitors.pdf",
      "link": "https://github.com/ghen/filedown/raw/master/test/resources/Bypass Capacitors.pdf"
    },
    { 
      "filename": "Ops.tmp",
      "link": "https://github.com/ghen/filedown/raw/master/test/resources/Ops.tmp"
    }
  ]
}
```

**Note:**
> As shown in the example above, same file link might be requested to be saved under the different names. Yet **filename** is always unique in scope of the request (job).

Response:

```
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
```

```json
{
   "id":"Vgjzu0q9SBqLFNJv0m13dw",
   "status":"Created",
   "threads":5,
   "files":[
      {
         "filename":"Solar System.jpg",
         "link":"https://github.com/ghen/filedown/raw/master/test/resources/Solar System.jpg"
      },
      {
         "filename":"Bypass Capacitors.pdf",
         "link":"https://github.com/ghen/filedown/raw/master/test/resources/Bypass Capacitors.pdf"
      },
      {
         "filename":"Ops.tmp",
         "link":"https://github.com/ghen/filedown/raw/master/test/resources/Ops.tmp"
      }
   ]
}
```
