# Exercise — Admins only

Add role-based access. `POST /login` is written for you: `sara` gets the role `admin`, and everyone else gets `user`.

1. Define a policy called `AdminOnly` that requires the `admin` role.
2. `GET /reports`: any logged-in user.
3. `DELETE /reports`: `AdminOnly` policy.
