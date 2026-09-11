# Module 2 Capstone — Library Lending System

No new concepts in this lesson — it's where everything from Module 2 comes together in one small but real system. This mirrors the university plan's Week 2 deliverable: **code applying SOLID, with solid test coverage**.

## What you're building

A library lending system: books that can be borrowed and returned, members with a borrow limit, and a configurable late-fee policy. Read the exercise `README.md` for the exact spec — this lesson page just points out which Module 2 concept shows up where, so you recognize the design decisions instead of just typing code.

## Where each lesson shows up

**Classes & encapsulation (lessons 01–02).** `Book.IsAvailable` has a `private set` — only `Book`'s own methods (`MarkBorrowed`/`MarkReturned`) can change it. Nothing outside can silently set a book to "available" without going through the rules.

**Exceptions (lesson 07).** Borrowing an unavailable book, or a member at their limit, raises specific custom exceptions (`BookNotAvailableException`, `BorrowLimitExceededException`) — not a generic `Exception`, and not a silent `false` return. These are genuine invalid operations, not routine "did it work" checks (recall lesson 07's distinction).

**SRP.** Three separate classes — `Book` (a single book's state), `Member` (a single member's borrowed items), `Library` (the catalog and lending rules) — instead of one class doing everything. Each has one reason to change.

**OCP + DIP, together.** `Library` takes an `ILateFeePolicy` through its constructor (lesson 01's constructors + lesson 04's interfaces + lesson 08's DIP). Want a different fee schedule — double fees during a promotion, no fees at all, a flat fee instead of per-day? Write a new class implementing `ILateFeePolicy`. `Library` itself never changes — that's OCP, achieved through DIP.

**Interfaces (lesson 04).** `ILateFeePolicy` is a pure capability contract — "can calculate a fee from days late" — with no assumptions about *how*.

**Collections (Module 1 + generics, lesson 06).** `Library` holds its catalog in a `Dictionary<string, Book>` keyed by ISBN for fast lookup; `Member` tracks borrowed ISBNs in a `List<string>`, exposed as `IReadOnlyList<string>` (lesson 02's encapsulation principle again — don't hand out a mutable list).

## Why this matters for the interview / evaluation

The plan's evaluation includes a technical interview. "Walk me through your design" is a near-certain question. Being able to say *"Library depends on an interface for fee calculation instead of a concrete class, so swapping the pricing rule doesn't require touching Library at all"* — and actually meaning it — is worth more than reciting the SOLID acronym.

## What to do

Open the exercise `README.md`. Build `LibrarySystem.cs`. When all tests pass, you've applied every Module 2 concept in one project — that's the actual capstone deliverable, not a separate write-up.
