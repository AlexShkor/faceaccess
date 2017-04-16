<template>
  <div>
    <div class="tile is-ancestor">
      <div class="tile is-parent is-6">
        <article class="tile is-child box">
          <h4 class="title">Add Faces</h4>
          <div class="content">
            <div>Пользователь {{ $route.params.id }}</div>
            <img id='img' :src="currentPhoto" alt="" style="width:400px;height:300px" />
            <canvas id="canvas" width="400" height="300"></canvas>
            <div class="block">
              <button class="button is-primary" @click='takePhoto'>Take Photo</button>
              <button class="button is-success" @click='upload'>Upload Current</button>
               <button class="button" @click='detect'>Detect</button>
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
  </div>
</template>

<script>
  import usersDs from 'components/UsersDataService'
  import VueWebcam from 'components/Webcam'

  export default {
    components: {
      VueWebcam
    },
    data() {
      return {
        currentPhoto: null,
        photos: []
      }
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
        usersDs.addFace(this.$route.params.id, this.currentPhoto.toString('base64')).then(() => {
          newPhoto.uploading = false;
        })
        this.photos.push(newPhoto);
      },
      detect() {
        usersDs.detectFace(this.$route.params.id, this.currentPhoto.toString('base64')).then((res) => {
          console.log(res)
          var rect = res.data.rect;

           var canvas = document.getElementById('canvas');
        var ctx = canvas.getContext('2d');
        ctx.beginPath();
ctx.lineWidth="2";
ctx.strokeStyle="green";
        ctx.rect(rect.left, rect.top, rect.width, rect.height)
        ctx.stroke();
        })
      }
    }
  }
</script>
