import lazyLoading from './lazyLoading'

export default {
name: 'User Profile',
    meta: {
    icon: 'fa fa-user-circle-o',
    expanded: false,
    isRequiresAuth: true,
    isAdmin: false
    },
children: [
{
    name: 'addFaces',
    path: '/users/'+localStorage.getItem('UserId'),
    meta: {
        label: 'Add Faces',
        isRequiresAuth: true
    },
    component: lazyLoading('userProfile/addFaces')
},
{
    name: 'profile',
    path: '/userProfile/profile/'+localStorage.getItem('UserId'),
    meta: {
        label: 'Profile',
        isRequiresAuth: true
    },
    component: lazyLoading('userProfile/profile')
}
]
}