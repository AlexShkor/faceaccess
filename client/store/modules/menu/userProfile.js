import lazyLoading from './lazyLoading'
import auth from 'components/AuthService'

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
    path: '/users/' + auth.getUserId(),
    meta: {
        label: 'Add Faces',
        isRequiresAuth: true
    },
    component: lazyLoading('userProfile/addFaces')
},
{
    name: 'profile',
    path: '/userProfile/profile/' + auth.getUserId(),
    meta: {
        label: 'Profile',
        isRequiresAuth: true
    },
    component: lazyLoading('userProfile/profile')
}
]
}