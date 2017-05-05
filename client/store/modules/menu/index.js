import * as types from '../../mutation-types'
import lazyLoading from './lazyLoading'
import tables from './tables'
import userProfile from './userProfile'

// show: meta.label -> name
// name: component name
// meta.label: display label

const state = {
  items: [
    {
      name: 'Dashboard',
      path: '/dashboard',
      meta: {
          icon: 'fa-tachometer',
          isRequiresAuth: true          
      },
      component: lazyLoading('dashboard', true)
    },
    {
      name: 'Users',
      path: '/users',
      meta: {
          icon: 'fa-users',
          isRequiresAuth: true,
          isAdmin: true
      },
      component: lazyLoading('users', true)
    },
    {
      name: 'Testing',
      path: '/testing',
      meta: {
          icon: 'fa-rocket',
          isRequiresAuth: true
      },
      component: lazyLoading('testing', true)
    },
    userProfile,
    tables
  ]
}

const mutations = {
  [types.EXPAND_MENU] (state, menuItem) {
    if (menuItem.index > -1) {
      if (state.items[menuItem.index] && state.items[menuItem.index].meta) {
        state.items[menuItem.index].meta.expanded = menuItem.expanded
      }
    } else if (menuItem.item && 'expanded' in menuItem.item.meta) {
      menuItem.item.meta.expanded = menuItem.expanded
    }

  }
}

export default {
  state,
  mutations
}
