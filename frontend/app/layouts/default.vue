<template>
  <div>
    <b-navbar>
      <template #brand>
        <b-navbar-item @click="$router.push('/')">
          <b-icon icon="chart-line"></b-icon>
          <strong class="ml-2">Backtest CPGR</strong>
        </b-navbar-item>
      </template>

      <template #end>
        <b-navbar-dropdown label="Ativo">
          <b-navbar-item @click="$router.push('/ativos/criar')">
            <b-icon icon="plus" size="is-small"></b-icon>
            <span class="ml-2">Criar</span>
          </b-navbar-item>
          <b-navbar-item @click="$router.push('/ativos')">
            <b-icon icon="format-list-bulleted" size="is-small"></b-icon>
            <span class="ml-2">Listar</span>
          </b-navbar-item>
        </b-navbar-dropdown>

        <b-navbar-dropdown label="DayTrade">
          <b-navbar-item @click="$router.push('/daytrades/criar')">
            <b-icon icon="plus" size="is-small"></b-icon>
            <span class="ml-2">Criar</span>
          </b-navbar-item>
          <b-navbar-item @click="$router.push('/daytrades')">
            <b-icon icon="format-list-bulleted" size="is-small"></b-icon>
            <span class="ml-2">Listar</span>
          </b-navbar-item>
        </b-navbar-dropdown>

        <b-navbar-dropdown label="Zona de Backtest">
          <b-navbar-item @click="$router.push('/backtests/criar')">
            <b-icon icon="calculator-variant" size="is-small"></b-icon>
            <span class="ml-2">Criar</span>
          </b-navbar-item>
          <b-navbar-item @click="$router.push('/backtests')">
            <b-icon icon="chart-box" size="is-small"></b-icon>
            <span class="ml-2">Listar</span>
          </b-navbar-item>
        </b-navbar-dropdown>
      </template>
    </b-navbar>

    <section class="section">
      <div class="container">
        <slot />
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
</script>
