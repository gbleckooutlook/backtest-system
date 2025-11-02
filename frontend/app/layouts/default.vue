<template>
  <div style="min-height: 100vh; background-color: #0f1419;">
    <Menubar :model="menuItems" style="margin-bottom: 0; border-radius: 0;">
      <template #start>
        <div style="display: flex; align-items: center; gap: 0.75rem; font-weight: 600; font-size: 1.25rem; color: #3b82f6;">
          <i class="pi pi-chart-line" style="font-size: 1.5rem;"></i>
          <span>Backtest CPGR</span>
        </div>
      </template>
      <template #end>
        <div class="menu-items">
          <a 
            v-for="item in menuItems" 
            :key="item.label"
            @click="toggleMenu(item)"
            class="menu-link"
          >
            <i :class="item.icon"></i>
            <span>{{ item.label }}</span>
            <i class="pi pi-angle-down menu-arrow"></i>
            
            <div v-if="item.showSubmenu" class="submenu">
              <NuxtLink 
                v-for="subitem in item.items" 
                :key="subitem.label"
                :to="subitem.to"
                class="submenu-item"
                :class="{ 'disabled': subitem.disabled }"
                @click.stop="item.showSubmenu = false"
              >
                <i :class="subitem.icon"></i>
                <span>{{ subitem.label }}</span>
              </NuxtLink>
            </div>
          </a>
        </div>
      </template>
    </Menubar>
    
    <div style="max-width: 1400px; margin: 0 auto; padding: 2.5rem 1.5rem;">
      <slot />
    </div>
    
    <Toast />
  </div>
</template>

<script setup lang="ts">
const menuItems = ref([
  {
    label: 'Ativo',
    icon: 'pi pi-chart-line',
    showSubmenu: false,
    items: [
      {
        label: 'Criar',
        icon: 'pi pi-plus',
        to: '/ativos/criar'
      },
      {
        label: 'Listar',
        icon: 'pi pi-list',
        to: '/ativos'
      }
    ]
  },
  {
    label: 'Backtest',
    icon: 'pi pi-calculator',
    showSubmenu: false,
    items: [
      {
        label: 'Criar',
        icon: 'pi pi-plus',
        disabled: true
      },
      {
        label: 'Listar',
        icon: 'pi pi-list',
        disabled: true
      }
    ]
  }
])

const toggleMenu = (item: any) => {
  // Fecha todos os outros menus
  menuItems.value.forEach(m => {
    if (m !== item) {
      m.showSubmenu = false
    }
  })
  // Toggle do menu atual
  item.showSubmenu = !item.showSubmenu
}

// Fecha menu ao clicar fora
onMounted(() => {
  const handleClickOutside = (e: MouseEvent) => {
    const target = e.target as HTMLElement
    if (!target.closest('.menu-items')) {
      menuItems.value.forEach(m => m.showSubmenu = false)
    }
  }
  
  document.addEventListener('click', handleClickOutside)
  
  // Cleanup
  onUnmounted(() => {
    document.removeEventListener('click', handleClickOutside)
  })
})
</script>

<style scoped>
.p-menubar {
  border-radius: 0 !important;
}

:deep(.p-menubar-root-list) {
  display: none !important;
}

.menu-items {
  display: flex;
  gap: 2rem;
  align-items: center;
}

.menu-link {
  position: relative;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  color: #e6e8eb;
  font-weight: 500;
  font-size: 0.95rem;
  cursor: pointer;
  border-radius: 6px;
  transition: all 0.2s ease;
  user-select: none;
}

.menu-link:hover {
  background-color: #252d3d;
  color: #3b82f6;
}

.menu-link i {
  font-size: 1rem;
}

.menu-arrow {
  font-size: 0.75rem !important;
  transition: transform 0.2s ease;
}

.submenu {
  position: absolute;
  top: calc(100% + 0.5rem);
  right: 0;
  min-width: 200px;
  background: #1e2533;
  border: 1px solid #2d3548;
  border-radius: 8px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.5);
  padding: 0.5rem;
  z-index: 1000;
  animation: slideDown 0.2s ease;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.submenu-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  color: #e6e8eb;
  text-decoration: none;
  border-radius: 6px;
  transition: all 0.2s ease;
  font-size: 0.9rem;
}

.submenu-item:hover:not(.disabled) {
  background-color: #252d3d;
  color: #3b82f6;
}

.submenu-item.disabled {
  opacity: 0.5;
  cursor: not-allowed;
  pointer-events: none;
}

.submenu-item i {
  font-size: 0.9rem;
}
</style>


