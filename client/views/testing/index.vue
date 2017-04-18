<template>
  <div class="content has-text-centered">
    <h1> Paralect FaceAccess System</h1>

     <vue-webcam ref='webcam'></vue-webcam>
        <hr/>
        <img :src="photo" alt="" style="width:400px;height:300px" />
        <hr/>
        <button type="button" @click="take_photo">Take Photo</button>
        <button type="button" @click="startStreaming">Start</button>
        <button type="button" @click="stopStreaming">Stop</button>
        <button type="button" @click="sendBlob">Send</button>
  </div>
</template>

<script>
import VueWebcam from 'components/Webcam'



var recordedChunks = [];
var mediaRecorder = null;

var  webSocketProtocol = location.protocol == "https:" ? "wss:" : "ws:";
            var webSocketURI = webSocketProtocol + "//" + location.host + "/ws";
            var socket = new WebSocket(webSocketURI);
            socket.onopen = function () {
                console.log("Connected.");
            };
            socket.onclose = function (event) {
                if (event.wasClean) {
                    console.log('Disconnected.');
                } else {
                    console.log('Connection lost.'); // for example if server processes is killed
                }
                console.log('Code: ' + event.code + '. Reason: ' + event.reason);
            };
            socket.onmessage = function (event) {
               recordedChunks.push(event.data);
                console.log("Data received: " + event.data);
            };
            socket.onerror = function (error) {
                console.log("Error: " + error.message);
            };

export default {
  components: { VueWebcam },
  data () {
    return {
      photo: null
    }
  },
  methods: {
    take_photo () {
      this.photo = this.$refs.webcam.getPhoto()
    },
    startStreaming(){
      const handleDataAvailable = (event) => {
        console.log(event)
        if (event.data.size > 0) {
          //recordedChunks.push(event.data);
          socket.send(event.data);
        } else {
         // ...
        }
      };

      navigator.getUserMedia({ video: true }, (stream) => {
          console.log(stream);
                var options = {mimeType: 'video/webm;codecs=vp9'};
      mediaRecorder = new MediaRecorder(stream, options);
      mediaRecorder.ondataavailable = handleDataAvailable;

      mediaRecorder.start();
        console.log(mediaRecorder.state);
  console.log("recorder started");
            }, (error) => {
                console.log(error);
            });



    },
    stopStreaming(){
      mediaRecorder.onstop = () =>{

      console.log(recordedChunks);
      var superBuffer = new Blob(recordedChunks);
      this.$refs.webcam.video.src = window.URL.createObjectURL(superBuffer);
      }
      mediaRecorder.stop();
    },
    sendBlob(){
      mediaRecorder.requestData()
    }
  }
}
</script>

<style lang="scss" scoped>
.is-title {
  text-transform: capitalize;
}
</style>
