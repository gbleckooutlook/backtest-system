<template>
  <div style="display: flex; align-items: center; justify-content: center; min-height: 65vh;">
    <Card style="width: 100%; max-width: 850px; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);">
      <template #title>
        <div style="text-align: center; padding: 1rem 0;">
          <div style="background: linear-gradient(135deg, #1e40af 0%, #3b82f6 100%); width: 80px; height: 80px; border-radius: 20px; display: flex; align-items: center; justify-content: center; margin: 0 auto 1.5rem;">
            <i class="pi pi-chart-line" style="font-size: 2.5rem; color: white;"></i>
          </div>
          <h1 style="font-size: 2.25rem; font-weight: 700; color: #0f172a; margin: 0; letter-spacing: -0.025em;">
            Bem-vindo ao Backtest CPGR
          </h1>
        </div>
      </template>
      <template #content>
        <p style="text-align: center; font-size: 1.125rem; color: #64748b; line-height: 1.8; margin-bottom: 2rem;">
          Sistema profissional de backtest para operações no mercado financeiro.
          <br />
          <br />
          Cadastre ativos e analise suas estratégias com precisão e eficiência.
        </p>
        <div style="display: flex; gap: 1rem; justify-content: center; flex-wrap: wrap;">
          <Button 
            label="Cadastrar Ativo" 
            icon="pi pi-plus" 
            @click="$router.push('/ativos/criar')"
            size="large"
            style="padding: 1rem 2rem !important; font-size: 1rem !important;"
          />
          <Button 
            label="Ver Ativos" 
            icon="pi pi-list" 
            severity="secondary"
            @click="$router.push('/ativos')"
            size="large"
            style="padding: 1rem 2rem !important; font-size: 1rem !important;"
          />
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})
</script>


