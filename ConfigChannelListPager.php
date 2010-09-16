<?php

class ConfigChannelListPager extends TablePager
{

	function getQueryInfo()
	{
		return array(
			'tables' => 'channellist',
			'fields' => array('name', 'network', 'enabled')
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
		'name' => "IRC Channel",
		'network' => "IRC Network",
		'enabled' => "Enabled/Disabled"
		);
	}

}
