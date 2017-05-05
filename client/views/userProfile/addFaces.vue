<template>
    <div>
        <div class="tile is-ancestor">
            <div class="tile is-parent is-6">
                <article class="tile is-child box">
                    <h4 class="title">Add Faces</h4>
                    <div class="content">
                        <div>Пользователь {{ $route.params.id }}</div>
                        <div>Имя {{ currentName }}</div>
                        <div>currentFaceId {{ currentFaceId }}</div>
                        <img id='img' :src="currentPhoto" alt="" style="width:400px;height:300px" />
                        <canvas id="canvas" width="400" height="300"></canvas>
                        <div class="block">
                            <button class="button is-primary" @click='takePhoto'>Take Photo</button>
                            <button class="button is-success" @click='upload'>Upload Current</button>
                            <button class="button" @click='detect'>Detect</button>
                            <button class="button" @click='train'>Train</button>
                            <button class="button is-danger" @click='identify'>Identify</button>
                        </div>
                    </div>
                </article>
            </div>
            <div class="tile is-parent is-6">
                <article class="tile is-child box">
                    <h4 class="title"></h4>
                    <div class="content">
                        <vue-webcam ref='webcam'></vue-webcam>
                    </div>
                </article>
            </div>
        </div>
        <div class="columns is-multiline">
            <div v-for='(photo, index) in photos' class="column is-one-quarter">
                <div>
                    <a class="level-item">
                        <span class="icon is-small">#{{index +1}} </span>
                    </a>
                    <nav class="level">
                        <div class="level-left">
                            <a class="level-item" title="Delete" @click='deleteFace(index, photo.id)'>
                                <span  class="icon is-small"><i class="fa fa-trash"></i></span>
                            </a>
                        </div>
                    </nav>
                    <div></div>
                </div>
                <img class="box" :title="photo.id" :src='photo.img || photo.imageBase64' />
            </div>
        </div>
    </div>
</template>

<script>
  import usersDs from 'components/UsersDataService'
  import VueWebcam from 'components/Webcam'

  var interval = null;
  const prefix = "data:image/jpeg;base64,";

  export default {
    components: {
      VueWebcam
    },
    data() {
      return {
        currentPhoto: null,
        currentFaceId: null,
        currentName: null,
        photos: []
      }
    },
    mounted(){
      usersDs.getFaces(this.$route.params.id).then((res)=>{
        this.photos = res.data;
      })
    },
    methods: {
      takePhoto() {
        this.currentPhoto = this.$refs.webcam.getPhoto();
        var canvas = document.getElementById('canvas');
        var ctx = canvas.getContext('2d');
        var img = new Image
        img.onload = function() {
          ctx.drawImage(img, 0, 0); // Or at whatever offset you like
        };
        img.src = this.currentPhoto;
      },
      upload() {
        var newPhoto = {
          img: this.currentPhoto,
          uploading: true
        };
        usersDs.addFace(this.$route.params.id, this.currentPhoto.toString('base64')).then((res) => {
          newPhoto.uploading = false;
          Object.assign(newPhoto, res.data)
        })
        this.currentPhoto = null;
        this.photos.push(newPhoto);
      },
      detect() {
        usersDs.detectFace(this.$route.params.id, this.currentPhoto.toString('base64')).then((res) => {
          console.log(res)
          var rect = res.data.rect;
          this.currentFaceId = res.data.faceId;
          var canvas = document.getElementById('canvas');
          var ctx = canvas.getContext('2d');
          ctx.beginPath();
          ctx.lineWidth = "2";
          ctx.strokeStyle = "green";
          ctx.rect(rect.left, rect.top, rect.width, rect.height)
          ctx.stroke();
          for (let key in res.data.landmarks) {
            let point = res.data.landmarks[key];
            console.log(point);
            ctx.beginPath();
            ctx.lineWidth = "2";
            ctx.strokeStyle = "blue";
            ctx.rect(point.x, point.y, 1, 1)
            ctx.stroke();
          }
        })
      },
      start() {
        interval = setInterval(() => {
          this.takePhoto();
          this.detect();
        }, 3000)
      },
      stop() {
        clearInterval(interval);
      },
      train() {
        usersDs.train();
      },
      identify() {
        this.currentName = '';
        usersDs.identify(this.currentFaceId).then((res) => {
          this.currentName = res.data.name;
        });
      },
      deleteFace(index ,photoId){
          usersDs.deletePhoto(photoId).then((response) => {
              if(response.data.statusCode === 200){
                  this.photos.splice(index, 1);
              }           
          });
      }
    }
  }
</script>