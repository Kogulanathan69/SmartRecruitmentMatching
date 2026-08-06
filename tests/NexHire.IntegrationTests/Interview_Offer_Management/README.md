# Member 5 integration-test scenarios

Create executable tests in the team's existing integration-test host for:

1. Company schedules an interview for its shortlisted application — 200.
2. Non-shortlisted application — 409.
3. Past schedule — 400.
4. Wrong company ownership — 403/404.
5. Candidate reads only their interview — 200; another candidate — 403/404.
6. Complete then record valid score — 200.
7. Score outside configured range — 400.
8. Create draft and send — Draft → Sent.
9. Candidate accepts their unexpired sent offer — Sent → Accepted.
10. Candidate cannot accept another candidate's, expired, withdrawn, or non-sent offer.
11. Company cannot withdraw another company's offer.
12. Duplicate evaluator score is rejected.
13. Problem Details includes stable `code` and `traceId`.
