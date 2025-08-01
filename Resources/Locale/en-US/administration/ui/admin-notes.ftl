# SPDX-FileCopyrightText: 2023 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
# SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
# SPDX-FileCopyrightText: 2023 Riggle <27156122+RigglePrime@users.noreply.github.com>
# SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
# SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
# SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
# SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
# SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
#
# SPDX-License-Identifier: MIT

# UI
admin-notes-title = Notes for {$player}
admin-notes-new-note = New note
admin-notes-show-more = Show more
admin-notes-for = Note for: {$player}
admin-notes-id = Id: {$id}
admin-notes-type = Type: {$type}
admin-notes-severity = Severity: {$severity}
admin-notes-secret = Secret
admin-notes-notsecret = Not secret
admin-notes-expires = Expires on: {$expires}
admin-notes-expires-never = Does not expire
admin-notes-edited-never = Never
admin-notes-round-id = Round Id: {$id}
admin-notes-round-id-unknown = Round Id: Unknown
admin-notes-created-by = Created by: {$author}
admin-notes-created-at = Created At: {$date}
admin-notes-last-edited-by = Last edited by: {$author}
admin-notes-last-edited-at = Last edited at: {$date}
admin-notes-edit = Edit
admin-notes-delete = Delete
admin-notes-hide = Hide
admin-notes-delete-confirm = Confirm delete
admin-notes-edited = Last edit by {$author} on {$date}
admin-notes-unbanned = Unbanned by {$admin} on {$date}
admin-notes-message-desc = [color=white]You have received { $count ->
    [1] an administrative message
    *[other] administrative messages
} since the last time you played on this server.[/color]
admin-notes-message-admin = From [bold]{ $admin }[/bold], written on { TOSTRING($date, "f") }:
admin-notes-message-wait = The accept button will be enabled after {$time} seconds.
admin-notes-message-accept = Dismiss permanently
admin-notes-message-dismiss = Dismiss for now
admin-notes-message-seen = Seen
admin-notes-banned-from = Banned from
admin-notes-the-server = the server
admin-notes-permanently = permanently
admin-notes-days = {$days} days
admin-notes-hours = {$hours} hours
admin-notes-minutes = {$minutes} minutes

# Note editor UI
admin-note-editor-title-new = Creating a new note for {$player}
admin-note-editor-title-existing = Editing note {$id} on {$player} by {$author}
admin-note-editor-pop-out = Pop out
admin-note-editor-secret = Secret?
admin-note-editor-secret-tooltip = Checking this will make the note not be visible by the player
admin-note-editor-type-note = Note
admin-note-editor-type-message = Message
admin-note-editor-type-watchlist = Watchlist
admin-note-editor-type-server-ban = Server Ban
admin-note-editor-type-role-ban = Role Ban
admin-note-editor-severity-select = Select
admin-note-editor-severity-none = None
admin-note-editor-severity-low = Low
admin-note-editor-severity-medium = Medium
admin-note-editor-severity-high = High
admin-note-editor-expiry-checkbox = Permanent?
admin-note-editor-expiry-checkbox-tooltip = Check this to make it expire
admin-note-editor-expiry-label = Expires in:
admin-note-editor-expiry-label-params = Expires on: {$date} (in {$expiresIn})
admin-note-editor-expiry-label-expired = Expired
admin-note-editor-expiry-placeholder = Enter expiration time (integer).
admin-note-editor-submit = Submit
admin-note-editor-submit-confirm = Are you sure?

# Time
admin-note-button-minutes = Minutes
admin-note-button-hours = Hours
admin-note-button-days = Days
admin-note-button-weeks = Weeks
admin-note-button-months = Months
admin-note-button-years = Years
admin-note-button-centuries = Centuries


# Verb
admin-notes-verb-text = Open Admin Notes

# Watchlist and message login
admin-notes-watchlist = Watchlist for {$player}: {$message}
admin-notes-new-message = You've received an admin message from {$admin}: {$message}
admin-notes-fallback-admin-name = [System]

# Admin remarks
admin-remarks-command-description = Opens the admin remarks page
admin-remarks-command-error = Admin remarks have been disabled
admin-remarks-title = Admin remarks

# Misc
system-user = [System]
