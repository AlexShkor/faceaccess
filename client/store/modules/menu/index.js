import * as types from '../../mutation-types'
import lazyLoading from './lazyLoading'
import tables from './tables'

// show: meta.label -> name
// name: component name
// meta.label: display label

const state = {
  items: [
    {
      name: 'Dashboard',
      path: '/dashboard',
      meta: {
        icon: 'fa-tachometer'
      },
      component: lazyLoading('dashboard', true)
    },
    {
      name: 'Users',
      path: '/users',
      meta: {
        icon: 'fa-users',
      },
      component: lazyLoading('users', true)
    },
    {
      name: 'Testing',
      path: '/testing',
      meta: {
        icon: 'fa-rocket',
      },
      component: lazyLoading('testing', true)
    },
    tables
  ],
  itemsForUser: [
  {
      name: 'Dashboard',
      path: '/dashboard',
      meta: {
          icon: 'fa-tachometer'
      },
      component: lazyLoading('dashboard', true)
  },
  {
      name: 'User profile',
      path: '/users/'+localStorage.getItem('UserId'),
      meta: {
          icon: 'fa fa-user-circle-o',
      },
      component: lazyLoading('../views/userProfile', true)
  },
  {
      name: 'Testing',
      path: '/testing',
      meta: {
          icon: 'fa-rocket',
      },
      component: lazyLoading('testing', true)
  },
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
