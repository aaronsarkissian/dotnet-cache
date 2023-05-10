# dotnet-cache

A simple dotnet-cache which uses MemoryCache.

## Endpoints

1. set cache

```bash
curl -X 'POST' \
  'http://localhost:8080/cache/set?duration=300' \
  -H 'Content-Type: application/json' \
  -d '{
  "value": "sample"
}'
```

2. get cache

```bash
curl -X 'GET' \
  'http://localhost:8080/cache/get/<GUID>'
```

3. count cached items

```bash
curl -X 'GET' \
  'http://localhost:8080/cache/count'
```

4. delete cache

```bash
curl -X 'DELETE' \
  'http://localhost:8080/cache/remove/<GUID>'
```