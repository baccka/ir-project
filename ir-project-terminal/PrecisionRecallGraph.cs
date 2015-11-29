using System;
using System.Collections.Generic;
using System.Linq;

namespace ir_project_terminal
{
    public class PrecisionRecallGraph
    {
        private static String source = @"
<!DOCTYPE html>
<html>
<head>
  <meta http-equiv='content-type' content='text/html; charset=UTF-8'>
  <title>Precision Recall graph</title>
  
  
  <script type='text/javascript' src='https://code.jquery.com/jquery-1.9.1.js'></script>

  
  <style type='text/css'>
    
  </style>
  

</head>
<body>
  <script src='https://code.highcharts.com/highcharts.js'></script>
<script src='https://code.highcharts.com/modules/exporting.js'></script>

<div id='container' style='min-width: 310px; height: 400px; margin: 0 auto'></div>

<script>
$(function () {
    $('#container').highcharts({
        title: {
            text: 'Precision/Recall Graph',
            x: -20 //center
        },
        subtitle: {
            text: 'For query # {{ID}}',
            x: -20
        },
        xAxis: {
            title: { text: 'Recall' }
        },
        yAxis: {
            title: {
                text: 'Precision'
            },
            plotLines: [{
                value: 0,
                width: 1,
                color: '#808080'
            }]
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle',
            borderWidth: 0
        },
        series: [{
            name: 'Precision/Recall',
            data: {{DATA}}
        }]
    });
});   
</script>

  
</body>

</html>
        ";
        
        public static void show(int queryId, IList<double> precision, IList<double> recall)
        {
            var data = "[ ";
            for (int i = 0; i < precision.Count; i++) 
            {
                if (i != 0)
                    data += ", ";
                data += "[" + recall[i].ToString() + ", " + precision[i].ToString() + "]";
            }
            data += " ]";
            var s = source.Replace("{{ID}}", queryId.ToString()).Replace("{{DATA}}", data);
            System.IO.File.WriteAllText("precisionRecall.html", s);
            // Open the graph in the browser.
            System.Diagnostics.Process.Start("precisionRecall.html");
        }
    }
}

