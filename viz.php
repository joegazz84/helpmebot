<?php
require_once 'MC/Google/Visualization.php';
require_once '../../AdminSettings.php';

$db = new PDO('mysql:dbname='.$wgDBname.';host='.$wgDBserver, $wgDBuser, $wgDBpassword);
$vis = new MC_Google_Visualization($db, 'mysql');

$vis->addEntity('hmb_accesslog', array(
    'fields' => array(
        'al_id' 			=> array('field' => 'al_id', 				'type' => 'number'	),
        'al_nuh' 			=> array('field' => 'al_nuh', 				'type' => 'text'	),
		'al_accesslevel' 	=> array('field' => 'al_accesslevel', 		'type' => 'text'	),
		'al_reqaccesslevel' => array('field' => 'al_reqaccesslevel', 	'type' => 'text'	),
		'al_date' 			=> array('field' => 'al_date', 				'type' => 'datetime'),
		'al_class' 			=> array('field' => 'al_class', 			'type' => 'text'	),
		'al_allowed' 		=> array('field' => 'al_allowed', 			'type' => 'boolean'	),
		'al_channel' 		=> array('field' => 'al_channel', 			'type' => 'text'	),
		'al_args' 			=> array('field' => 'al_args', 				'type' => 'text'	)
    )
));

$vis->handleRequest();
?>