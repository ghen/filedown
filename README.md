# File Downloader

**FileDownloader** is a simple HTTP REST SPI service built with .NET Core 3.x, that allows clients to schedule and track multiple files download jobs.

## Requirements

* **Windows 7 (64 bit)** or higher
* **.NET Core 3.1 Runtime** or later

## Clone

## Build

## Test

## Run

For local testing (and production troubleshooting) application can be run in **console mode**. In this mode application will run from current working directory (affects relative paths resolution if used in configuration), and will terminate immediately upon current console session completion.

* To run application in **console mode** use ***--console*** command line switch:

  ```
  > filedown --console
  ```

* To exit application use ***Ctrl+C*** control sequence, or terminate current console session.

**Note:**
> During development application can be run from **Microsoft Visual Studio** IDE directly. It will execute in **console mode** by default.

## Service Installation

Application can be installed as a local service using scripts provided in **./scripts**. No special tools are needed (other than **.NET Core 3.1 Runtime** being installed). User would also require elevated rights in order to install or uninstall service under the **Windows** operating system.

## Configuration

Application can be configured in all supported **.NET Core 3.1** manners. The easiest way would be to alter **./filedown.json** file:

```json
{
  "url": "http://localhost:8080/api",
  "jobs": 3,
  "threads": 3,
  "downloads": "./downloads"
}
```

Supported settings:

| Path			     | Required | Type        | Description |
| -------------- |:--------:|:-----------:| ----------- |
| **url**	       | Req      | String(1024) | HTTP REST API path to listen for incoming WEB requests for. |
| **jobs**	     | Req      | Number      | Number of maximum allowed parallel jobs. |
| **threads**    | Req      | Number      | Default number of threads per job. |
| **downloads**  | Req      | String(1024) | Path to the local folder to save downloaded files into. |

## Logs

**Note:**
> Default log files location is **./logs**.

Structured logs are sent to console (in console mode only), as well as saved to local file system as instructed in **./nlog.config** file. Application should also be configured to have sufficient privileges to access logs files location.

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
GET /api/jobs/E1HKfn68Pkms5zsZsvKONw HTTP/1.1
Accept: application/json
```

Response:

```
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
```

```json
{
  "id": "E1HKfn68Pkms5zsZsvKONw==",
  "status": "Processing",
  "files": [
    { 
      "filename": "picture.jpg", 
      "link": "http://example.com/image.jpg",
      "started": "2020-03-03T15:27:50.034+03:00",
      "finished": "2020-03-03T15:27:53.345+03:00",
      "size": 1024,
      "bytes": 1024
    },
    {
      "filename": "archive1.zip",
      "link": "http://example.com/archive.zip",
      "started": "2020-03-03T15:27:50.123+03:00",
      "finished": "2020-03-03T15:27:53.354+03:00",
      "size": 10240,
      "error": "HTTP 404 - Not Found"
    },
    {
      "filename": "archive2.zip",
      "link": "http://example.com/archive.zip",
      "started": "2020-03-03T15:27:50.678+03:00",
      "size": 10240,
    }
  ]
}
```

**Note:**
> As shown in the example above:
> * **picture.jpg** has been downloaded successfully
> * **archive1.zip** download failed with error
> * **archive2.zip** download is still in progress

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
    { "filename": "picture.jpg", "link": "http://example.com/image.jpg" },
    { "filename": "archive1.zip", "link": "http://example.com/archive.zip" },
    { "filename": "archive2.zip", "link": "http://example.com/archive.zip" }
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
  "id": "E1HKfn68Pkms5zsZsvKONw",
  "status": "Created",
  "files": [
    { "filename": "picture.jpg", "link": "http://example.com/image.jpg" },
    { "filename": "archive1.zip", "link": "http://example.com/archive.zip" },
    { "filename": "archive2.zip", "link": "http://example.com/archive.zip" }
  ]
}
```
