import jwtDecode from 'jwt-decode'

export default {
    getIsAuth() {
        try {
            var decode = jwtDecode(localStorage.getItem('Token'));
            if (decode != null) {
                return true;
            }        
        }
        catch (error) {
            return false;
        }       
    },
    getIsAdmin() {
        try {
            var decode = jwtDecode(localStorage.getItem('Token'));
            if (decode.role === 'USER') {
                return false;
            }
            if (decode.role === 'ADMIN') {
                return true;
            }
        }
        catch (error) {
            return error.name + ":" + error.message;
        }
    },
    getUserId() {
        try {
            var decode = jwtDecode(localStorage.getItem('Token'));
            return decode.id;
        }
        catch (error) {
            return null;
        }
    }

}