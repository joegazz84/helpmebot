<?php

#ini_set('display_errors',1);

if(!defined('MEDIAWIKI')) {
	echo("This file is an extension to the mediawiki software and cannot be used standalone\n");
	die( 1 );
}

define('NS_COMMAND', 204);
define('NS_COMMAND_TALK', 205);
define('NS_MESSAGE', 206);
define('NS_MESSAGE_TALK', 207);

$wgExtensionCredits['specialpage'][] = array(
'path' => __FILE__,
'name' => 'Helpmebot Viewer',
'description' => 'Allows viewing of Helpmebot\'s configuration etc.',
'author' => 'Simon Walker'
);

$wgAvailableRights[] = 'helpmebot-editmessages';
$wgAvailableRights[] = 'helpmebot-view-ignorelist';

$wgExtraNamespaces[NS_COMMAND] = "Command";
$wgExtraNamespaces[NS_COMMAND_TALK] = "Command_talk:";
$wgExtraNamespaces[NS_MESSAGE] = "Message";
$wgExtraNamespaces[NS_MESSAGE_TALK] = "Message_talk:";

$wgNamespaceProtection[NS_MESSAGE]      = array( 'helpmebot-editmessages' );

$wgAutoloadClasses['SpecialAccessList'] = $IP . '/extensions/Helpmebot/SpecialAccessList.php';
$wgAutoloadClasses['AccessListPager'] = $IP . '/extensions/Helpmebot/AccessListPager.php';

$wgAutoloadClasses['SpecialBrain'] = $IP.'/extensions/Helpmebot/SpecialBrain.php';
$wgAutoloadClasses['BrainPager'] = $IP.'/extensions/Helpmebot/BrainPager.php';

$wgAutoloadClasses['CommandListPager'] = $IP.    '/extensions/Helpmebot/CommandListPager.php';
$wgAutoloadClasses['SpecialCommandList'] = $IP . '/extensions/Helpmebot/SpecialCommandList.php';

$wgAutoloadClasses['SpecialHelpmebotConfiguration'] = $IP . '/extensions/Helpmebot/SpecialHelpmebotConfiguration.php';
$wgAutoloadClasses['ConfigChannelListPager'] = $IP.    '/extensions/Helpmebot/ConfigChannelListPager.php';


$wgAutoloadClasses['SpecialSubversionRepositories'] = $IP.'/extensions/Helpmebot/SpecialSubversionRepositories.php';

$wgSpecialPages['AccessList'] = 'SpecialAccessList';
$wgSpecialPages['Brain'] = 'SpecialBrain';
$wgSpecialPages['CommandList'] = 'SpecialCommandList';
$wgSpecialPages['HelpmebotConfiguration'] = 'SpecialHelpmebotConfiguration';
$wgSpecialPages['SubversionRepositories'] = 'SpecialSubversionRepositories';

$wgSpecialPageGroups['AccessList'] = 'helpmebot';
$wgSpecialPageGroups['Brain']="helpmebot";
$wgSpecialPageGroups['CommandList']="helpmebot";
$wgSpecialPageGroups['HelpmebotConfiguration']="helpmebot";
$wgSpecialPageGroups['SubversionRepositories']="developer";


$wgExtensionMessagesFiles['Helpmebot'] = $IP . '/extensions/Helpmebot/Helpmebot.i18n.php';

$wgHelpmebotStyleVersion=6;