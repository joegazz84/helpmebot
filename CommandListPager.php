<?php

class CommandListPager extends TablePager
{

	function getQueryInfo()
	{
		return array(
			'tables' => 'hmb_commands',
			'fields' => array('command','description', 'switches', 'accesslevel')
			);
	}

	function getIndexField(){return "command";}
	
	function getRowClass($row){return "commandlistentry-" . $row->accesslevel; }

	function isFieldSortable( $field )
	{ return false; }

	function formatValue( $name, $value )
	{
		if($name == "command")
		{
			$cmdpage = Title::makeTitle(NS_COMMAND, $value);
			
			return $this->getSkin()->link($cmdpage,$cmdpage->getText() );
			
		}
		
		return $value;
	}

	function getDefaultSort()
	{
		return ""; //user_accesslevel";
	}

	function getFieldNames()
	{
		return array(
		
		'command' => "Command",
		'description' => "Description",
		'switches' => "Switches",
		'accesslevel' => "Access Level"
		);
	}

}
