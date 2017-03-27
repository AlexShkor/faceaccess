<template>
  <div>
     <div>Пользователь {{ $route.params.id }}</div>
  </div>
</template>
<script>
    export default {
      data() {
        return {
          users: [{
            Name: "First",
            Id: "1"
          }, {
            Name: "Test",
            Id: "2"
          }]
        }
      },
      methods: {
        view() {

        }
      }
    }


</script>
