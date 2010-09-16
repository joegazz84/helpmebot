<?php

class ConfigChannelListPager extends TablePager
{

	function getQueryInfo()
	{
		return array(
			'tables' => 'channellist',
			'fields' => array('channel', 'network', 'enabled')
			);
	}

	function getIndexField(){return "id";}

	function getRowClass($row){return "channelentry-" . $row->enabled; }

	function isFieldSortable( $field )
	{ return false; }

	function formatValue( $name, $value )
	{
		return $value;
	}

	function getDefaultSort()
	{
		return ""; //user_accesslevel";
	}

	function getFieldNames()
	{
		return array(
		'channel' => "IRC Channel",
		'network' => "IRC Network",
		'enabled' => "Enabled/Disabled"
		);
	}

}
