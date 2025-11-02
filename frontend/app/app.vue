<template>
  <NuxtLayout>
    <NuxtPage />
  </NuxtLayout>
</template>

<style>
/* Tema Dark - Backtest CPGR */
:root {
  --background-dark: #1a1a1a;
  --surface-dark: #2d2d2d;
  --text-light: #e0e0e0;
  --border-dark: #404040;
}

html, body {
  background-color: var(--background-dark) !important;
  color: var(--text-light) !important;
}

.navbar {
  background-color: var(--surface-dark) !important;
  border-bottom: 1px solid var(--border-dark);
}

.navbar-item,
.navbar-link,
.navbar-dropdown .navbar-item {
  color: var(--text-light) !important;
}

.navbar-dropdown {
  background-color: var(--surface-dark) !important;
  border: 1px solid var(--border-dark);
}

.navbar-item:hover,
.navbar-link:hover,
.navbar-dropdown .navbar-item:hover {
  background-color: rgba(255, 255, 255, 0.1) !important;
}

.card,
.box {
  background-color: var(--surface-dark) !important;
  color: var(--text-light) !important;
  border: 1px solid var(--border-dark);
}

.title,
.subtitle {
  color: var(--text-light) !important;
}

.input,
.textarea,
.select select {
  background-color: #3a3a3a !important;
  border-color: var(--border-dark) !important;
  color: var(--text-light) !important;
}

.select select option {
  background-color: #2d2d2d !important;
  color: #e0e0e0 !important;
}

.input::placeholder,
.textarea::placeholder {
  color: #999 !important;
}

.label {
  color: var(--text-light) !important;
}

.help {
  color: #aaa !important;
}

.table {
  background-color: var(--surface-dark) !important;
  color: var(--text-light) !important;
}

.table thead th {
  background-color: #3a3a3a !important;
  color: var(--text-light) !important;
  border-color: var(--border-dark) !important;
}

.table td,
.table th {
  border-color: var(--border-dark) !important;
  color: var(--text-light) !important;
}

.table tbody tr {
  background-color: var(--surface-dark) !important;
}

.table tbody tr:nth-child(even) {
  background-color: #252d3d !important;
}

.table tbody tr:hover {
  background-color: rgba(255, 255, 255, 0.1) !important;
}

.pagination-link,
.pagination-previous,
.pagination-next {
  background-color: var(--surface-dark) !important;
  border-color: var(--border-dark) !important;
  color: var(--text-light) !important;
}

.pagination-link:hover,
.pagination-previous:hover,
.pagination-next:hover {
  border-color: #7a7a7a !important;
}

.pagination-link.is-current {
  background-color: #3273dc !important;
  border-color: #3273dc !important;
  color: white !important;
}

.file-cta {
  background-color: #3a3a3a !important;
  border-color: var(--border-dark) !important;
  color: var(--text-light) !important;
}

.file-cta:hover {
  background-color: #4a4a4a !important;
}

.section {
  background-color: var(--background-dark) !important;
}

.hero {
  background-color: var(--background-dark) !important;
}

/* Modal Dialog Dark Theme */
.modal-background {
  background-color: rgba(0, 0, 0, 0.7) !important;
}

.modal-card {
  background-color: var(--surface-dark) !important;
}

.modal-card-head {
  background-color: #3a3a3a !important;
  border-bottom: 1px solid var(--border-dark) !important;
}

.modal-card-title {
  color: var(--text-light) !important;
}

.modal-card-body {
  background-color: var(--surface-dark) !important;
  color: var(--text-light) !important;
}

.modal-card-foot {
  background-color: #3a3a3a !important;
  border-top: 1px solid var(--border-dark) !important;
}

.dialog .modal-card-body {
  color: var(--text-light) !important;
}

.dialog .modal-card-body .media-content {
  color: var(--text-light) !important;
}

.dialog .modal-card-body strong {
  color: #fff !important;
}

/* Toast Dark Theme */
.notices .toast {
  background-color: var(--surface-dark) !important;
  color: var(--text-light) !important;
  border: 1px solid var(--border-dark);
}

.notices .toast.is-success {
  background-color: #48c774 !important;
  color: white !important;
}

.notices .toast.is-danger {
  background-color: #f14668 !important;
  color: white !important;
}

.notices .toast.is-warning {
  background-color: #ffe08a !important;
  color: rgba(0, 0, 0, 0.7) !important;
}
</style>
