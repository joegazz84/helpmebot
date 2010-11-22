      google.load("visualization", "1", {packages:["piechart"]});
      google.setOnLoadCallback(drawChart);
      function drawChart() {
        //Tell Google Visualization where your script is
        var query = new google.visualization.Query('/vis.php');
        query.setQuery('select al_class, count(*) from hmb_accesslog group by al_class');
        query.send(function(result) {
          if(result.isError()) {
            alert(result.getDetailedMessage());
          } else {
            var chart = new google.visualization.PieChart(document.getElementById('chart_div'));
            chart.draw(result.getDataTable(), {width: 400, height: 240});
          }
        });
      }