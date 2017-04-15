import DataService from './DataService'

export default {
  getAll() {
    return DataService.get("/users");
  }
}
