import axios from 'axios'

const baseUrl = location.origin + '/api'

export default {
  get(url) {
    return axios.get(baseUrl + url);
  },
  post(url, data){
    return axios.post(baseUrl + url, data, {
      headers: {'Content-Type': 'application/json'}
    });
  },
  put(url, data){
    return axios.put(baseUrl + url, data);
  },
  delete(url){
    return axios.delete(baseUrl + url);
  }
}
