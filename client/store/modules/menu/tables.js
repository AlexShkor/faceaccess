import lazyLoading from './lazyLoading'

export default {
  name: 'Tables',
  meta: {
    icon: 'fa-table',
    expanded: false,
    isRequiresAuth: true
  },

  children: [
    {
      name: 'BasicTables',
      path: '/tables/basic',
      meta: {
          label: 'Basic Tables',
          isRequiresAuth: true
      },
      component: lazyLoading('tables/Basic')
    }
  ]
}
