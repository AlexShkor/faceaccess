import DataService from './DataService'

export default {
  getAll() {
    return DataService.get("/users");
  },
  createUser(data){
    return DataService.post("/users", data);
  },
  addFace(userId, photo){
    return DataService.post("/users/faces", {
      userId: userId,
      photo: photo
    });
  },
  detectFace(userId, photo){
    return DataService.post("/users/detect", {
      userId: userId,
      photo: photo
    });
  }
}
