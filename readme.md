# API Endpoint Testing Checklist

## Endpoint Details

- **Method**: POST
- **Base URL**: https://test.lan
- **Endpoint**: /client
- **Request Headers**:
  ```json
  {
    "Content-Type": "application/json"
  }
  ```
- **Request Body**:
  ```json
  {
    "id": 1,
    "name": "test"
  }
  ```
- **Response**:
  Status Code 200
  ```json
  {
    "name": "test",
    "age": 1,
    "adi": "addition_info"
  }
  ```

## Test Cases

| Test Case ID | Test Case Description                                                               | Expected Result                                                                                  |
| ------------ | ----------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------ |
| TC01         | Verify the endpoint with valid request body                                         | Status code 200, response body contains "name": "test", "age": 1, "adi": "addition_info"         |
| TC02         | Verify the endpoint with missing "id" in the request body                           | Status code 400, response body: {"error": "Missing required field 'id'"}                         |
| TC03         | Verify the endpoint with missing "name" in the request body                         | Status code 400, response body: {"error": "Missing required field 'name'"}                       |
| TC04         | Verify the endpoint with invalid "id" (e.g., non-integer value)                     | Status code 400, response body: {"error": "'id' must be an integer"}                             |
| TC05         | Verify the endpoint with invalid "name" (e.g., empty string)                        | Status code 400, response body: {"error": "'name' cannot be empty"}                              |
| TC06         | Verify the endpoint with "Content-Type" other than "application/json"               | Status code 415, response body: {"error": "Unsupported Media Type. Expected 'application/json'"} |
| TC07         | Verify the endpoint with an excessively long "name" value                           | Status code 400, response body: {"error": "'name' exceeds maximum length"}                       |
| TC08         | Verify the endpoint without "Content-Type" header                                   | Status code 400 or 415, response body: {"error": "Content-Type header is missing or invalid"}    |
| TC09         | Verify the endpoint with additional unexpected fields in the request body           | Status code 200, response body contains only expected fields                                     |
| TC10         | Verify the endpoint with valid request body but invalid method (e.g., GET)          | Status code 405, response body: {"error": "Method Not Allowed"}                                  |
| TC11         | Verify the response time of the endpoint                                            | Response time within acceptable limits (e.g., < 500ms)                                           |
| TC12         | Verify the response headers for the endpoint                                        | Appropriate headers like "Content-Type": "application/json" present in the response              |
| TC13         | Verify the endpoint with SQL injection attempt in the "name" field                  | Status code 400 or 500, response body: {"error": "Invalid input"}; no SQL injection              |
| TC14         | Verify the endpoint with boundary values for "id" (e.g., 0, max integer)            | Status code 200 or 400, appropriate response based on business logic                             |
| TC15         | Verify the endpoint with valid request body multiple times to check for idempotency | Status code 200, same response body each time                                                    |
| TC16         | Verify the response body schema for the endpoint                                    | Response body schema matches expected schema: {"name": string, "age": integer, "adi": string}    |
