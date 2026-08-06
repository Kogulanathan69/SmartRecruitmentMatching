# Member 5 API Matrix

| Method | Route | Actor | Purpose |
|---|---|---|---|
| POST | `/api/Interviews` | Company | Schedule interview for eligible application |
| GET | `/api/Interviews/company` | Company | Company-owned interviews |
| GET | `/api/Interviews/candidate` | Candidate | Candidate-owned interviews |
| GET | `/api/Interviews/{id}` | Owner | Interview details |
| PUT | `/api/Interviews/{id}/reschedule` | Company | Reschedule active interview |
| POST | `/api/Interviews/{id}/cancel` | Company | Cancel with reason |
| POST | `/api/Interviews/{id}/complete` | Company | Mark completed |
| POST | `/api/Interviews/{id}/score` | Company | Record evaluator score and feedback |
| POST | `/api/Offers` | Company | Create draft offer |
| GET | `/api/Offers/company` | Company | Company-owned offers |
| GET | `/api/Offers/candidate` | Candidate | Candidate-owned offers |
| GET | `/api/Offers/{id}` | Owner | Offer details |
| POST | `/api/Offers/{id}/send` | Company | Draft → Sent |
| POST | `/api/Offers/{id}/accept` | Candidate | Sent → Accepted |
| POST | `/api/Offers/{id}/reject` | Candidate | Sent → Rejected |
| POST | `/api/Offers/{id}/withdraw` | Company | Sent → Withdrawn |

## Cross-module contract

`IApplicationAccessReader` must be implemented by the Applications owner. It is read-only and must enforce the canonical current status and ownership data source.

## Error format

All Member 5 endpoints return RFC-style Problem Details with stable `code` and `traceId` extensions for 400, 403, 404, 409, 500, and 503 outcomes.
