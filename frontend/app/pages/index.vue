<template>
  <div class="hero is-medium">
    <div class="hero-body has-text-centered">
      <b-icon icon="chart-line" size="is-large" type="is-primary" class="mb-5"></b-icon>
      
      <h1 class="title is-1">Bem-vindo ao Backtest CPGR</h1>
      <p class="subtitle is-4">Sistema de backtesting para análise e validação de estratégias de trading</p>
      
      <div class="buttons is-centered mt-6">
        <b-button type="is-primary" size="is-large" disabled icon-left="calculator" class="mr-3">
          Criar Backtest
        </b-button>
        
        <b-button type="is-info" size="is-large" disabled icon-left="format-list-bulleted" class="ml-3">
          Listar Backtest
        </b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})
</script>
