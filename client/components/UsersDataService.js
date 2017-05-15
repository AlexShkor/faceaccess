import DataService from './DataService'

export default {
  getAll() {
    return DataService.get("api/users");
  },
  getFaces(userId) {
      return DataService.get("api/users/" + userId + "/faces");
  },
  addFace(userId, photos){
      return DataService.post("api/users/faces", {
      userId: userId,
      photos: photos
    });
  },
  detectFace(userId, photo){
      return DataService.post("api/users/detect", {
      userId: userId,
      photo: photo
    });
  },
  loadImages(userId){
      return DataService.get('api/users/{userid}/faces');
  },
  train() {
      return DataService.post('api/users/train', {});
  },
  identify(faceId) {
      return DataService.post('api/users/identify', { faceId: faceId });
  },
  deletePhoto(photoId) {
      return DataService.get("api/users/" + photoId + "/deletePhoto");
  }
}
